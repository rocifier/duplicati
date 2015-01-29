#region Disclaimer / License
// Copyright (C) 2011, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Duplicati.Library.Interface;

namespace Duplicati.Library.Backend
{
    public class S3 : IBackend, IStreamingBackend, IRenameEnabledBackend
    {
        //public const string RRS_OPTION = "s3-use-rrs";
        //public const string EU_BUCKETS_OPTION = "s3-european-buckets";
        //public const string SUBDOMAIN_OPTION = "s3-use-new-style";
        //public const string SERVER_NAME = "s3-server-name";
        public const string LOCATION_OPTION = "s3-location-constraint";
        public const string EMAIL_OPTION = "s3-email"; // actually the aws username
        //public const string SSL_OPTION = "use-ssl";

        //Updated list: http://docs.amazonwebservices.com/general/latest/gr/rande.html#s3_region
        public static readonly KeyValuePair<string, string>[] KNOWN_S3_LOCATIONS = new KeyValuePair<string, string>[] {
            new KeyValuePair<string, string>("Ireland", "eu-west-1"),
            new KeyValuePair<string, string>("Frankfurt", "eu-central-1"),
            new KeyValuePair<string, string>("Virginia", "us-east-1"),
            new KeyValuePair<string, string>("California", "us-west-1"),
            new KeyValuePair<string, string>("Oregon", "us-west-2"),
            new KeyValuePair<string, string>("Singapore", "ap-southeast-1"),
            new KeyValuePair<string, string>("Sydney", "ap-southeast-2"),
            new KeyValuePair<string, string>("Tokyo", "ap-northeast-1"),
            new KeyValuePair<string, string>("Sao Paulo", "sa-east-1"),
        };

        private string m_bucket;
        private string m_prefix;

        public const string DEFAULT_S3_HOST  = "s3.amazonaws.com";

        private Dictionary<string, string> m_options;

        private S3Wrapper m_wrapper;


        public S3()
        {
        }


        public S3(string url, Dictionary<string, string> options)
        {
            // make sure there is a host even though it's bogus,
            // so we can make sure the path is retrieved properly
            var uri = new Utility.Uri(url.Replace("s3://", "s3://c.c"));
            //uri.RequireHost();

            //string host = uri.Host;

            string awsID = null;
            string awsKey = null;

            if (options.ContainsKey("auth-username"))
                awsID = options["auth-username"];
            if (options.ContainsKey("auth-password"))
                awsKey = options["auth-password"];

            if (options.ContainsKey("aws_access_key_id"))
                awsID = options["aws_access_key_id"];
            if (options.ContainsKey("aws_secret_access_key"))
                awsKey = options["aws_secret_access_key"];
            if (!string.IsNullOrEmpty(uri.Username))
                awsID = uri.Username;
            if (!string.IsNullOrEmpty(uri.Password))
                awsKey = uri.Password;

            if (string.IsNullOrEmpty(awsID))
                throw new Exception(Strings.S3Backend.NoAMZUserIDError);
            if (string.IsNullOrEmpty(awsKey))
                throw new Exception(Strings.S3Backend.NoAMZKeyError);

            string locationConstraint;
            options.TryGetValue(LOCATION_OPTION, out locationConstraint);
            string emailUsername;
            options.TryGetValue(EMAIL_OPTION, out emailUsername);

            // The new simplified url style is s3://bucket/prefix
            //m_bucket = host;
            //host = s3host;
            m_bucket = "cbd-backup-" + locationConstraint; // use account id as part of bucket name
            m_prefix = emailUsername + "/v1/";
            m_options = options;

            //m_prefix = m_prefix.Trim();
            //if (m_prefix.Length != 0 && !m_prefix.EndsWith("/"))
            //    m_prefix += "/";

            m_wrapper = new S3Wrapper(awsID, awsKey, locationConstraint);
        }

        public static bool IsValidHostname(string bucketname)
        {
            if (string.IsNullOrEmpty(bucketname))
                return false;
            else
                return Amazon.S3.Util.AmazonS3Util.ValidateV2Bucket(bucketname);
        }

        #region IBackend Members

        public string DisplayName
        {
            get { return Strings.S3Backend.DisplayName; }
        }

        public string ProtocolKey
        {
            get { return "s3"; }
        }

        public bool SupportsStreaming
        {
            get { return true; }
        }

        public List<IFileEntry> List()
        {
            try
            {
                List<IFileEntry> lst = Connection.ListBucket(m_bucket, m_prefix);
                for (int i = 0; i < lst.Count; i++)
                {
                    ((FileEntry)lst[i]).Name = lst[i].Name.Substring(m_prefix.Length);

                    //Fix for a bug in Duplicati 1.0 beta 3 and earlier, where filenames are incorrectly prefixed with a slash
                    /*
                    if (lst[i].Name.StartsWith("/") && !m_prefix.StartsWith("/"))
                        ((FileEntry)lst[i]).Name = lst[i].Name.Substring(1);
                     */
                }
                return lst;
            }
            catch (Exception ex)
            {
                //Catch "non-existing" buckets
                Amazon.S3.AmazonS3Exception s3ex = ex as Amazon.S3.AmazonS3Exception;
                if (s3ex != null && (s3ex.StatusCode == System.Net.HttpStatusCode.NotFound || "NoSuchBucket".Equals(s3ex.ErrorCode)))
                    throw new Interface.FolderMissingException(ex);

                throw;
            }
        }

        public void Put(string remotename, string localname)
        {
            using (System.IO.FileStream fs = System.IO.File.Open(localname, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                Put(remotename, fs);
        }

        public void Put(string remotename, System.IO.Stream input)
        {
            try
            {
                Connection.AddFileStream(m_bucket, GetFullKey(remotename), input);
            }
			catch (Exception ex)
			{
                //Catch "non-existing" buckets
                Amazon.S3.AmazonS3Exception s3ex = ex as Amazon.S3.AmazonS3Exception;
                if (s3ex != null && (s3ex.StatusCode == System.Net.HttpStatusCode.NotFound || "NoSuchBucket".Equals(s3ex.ErrorCode)))
                    throw new Interface.FolderMissingException(ex);

                throw;
            }
        }

        public void Get(string remotename, string localname)
        {
            using (System.IO.FileStream fs = System.IO.File.Open(localname, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                Get(remotename, fs);
        }

        public void Get(string remotename, System.IO.Stream output)
        {
            Connection.GetFileStream(m_bucket, GetFullKey(remotename), output);
        }

        public void Delete(string remotename)
        {
            Connection.DeleteObject(m_bucket, GetFullKey(remotename));
        }

        public IList<ICommandLineArgument> SupportedCommands
        {
            get
            {
                StringBuilder hostnames = new StringBuilder();
                StringBuilder locations = new StringBuilder();
                //foreach(KeyValuePair<string, string> s in KNOWN_S3_PROVIDERS)
                //    hostnames.AppendLine(string.Format("{0}: {1}", s.Key, s.Value));

                foreach (KeyValuePair<string, string> s in KNOWN_S3_LOCATIONS)
                    locations.AppendLine(string.Format("{0}: {1}", s.Key, s.Value));

                return new List<ICommandLineArgument>(new ICommandLineArgument[] {
                    new CommandLineArgument("aws_secret_access_key", CommandLineArgument.ArgumentType.Password, Strings.S3Backend.AMZKeyDescriptionShort, Strings.S3Backend.AMZKeyDescriptionLong, null, new string[] {"auth-password"}, null),
                    new CommandLineArgument("aws_access_key_id", CommandLineArgument.ArgumentType.String, Strings.S3Backend.AMZUserIDDescriptionShort, Strings.S3Backend.AMZUserIDDescriptionLong, null, new string[] {"auth-username"}, null),
                    //new CommandLineArgument(SUBDOMAIN_OPTION, CommandLineArgument.ArgumentType.Boolean, Strings.S3Backend.S3NewStyleDescriptionShort, Strings.S3Backend.S3NewStyleDescriptionLong, "true", null, null, Strings.S3Backend.S3NewStyleDeprecation),
                    //new CommandLineArgument(RRS_OPTION, CommandLineArgument.ArgumentType.Boolean, Strings.S3Backend.S3UseRRSDescriptionShort, Strings.S3Backend.S3UseRRSDescriptionLong, "false"),
                    //new CommandLineArgument(SERVER_NAME, CommandLineArgument.ArgumentType.String, Strings.S3Backend.S3ServerNameDescriptionShort, string.Format(Strings.S3Backend.S3ServerNameDescriptionLong, hostnames.ToString()), DEFAULT_S3_HOST),
                    new CommandLineArgument(LOCATION_OPTION, CommandLineArgument.ArgumentType.String, Strings.S3Backend.S3LocationDescriptionShort, string.Format(Strings.S3Backend.S3LocationDescriptionLong, locations.ToString())),
                    //new CommandLineArgument(SSL_OPTION, CommandLineArgument.ArgumentType.Boolean, Strings.S3Backend.DescriptionUseSSLShort, Strings.S3Backend.DescriptionUseSSLLong),
                    new CommandLineArgument("auth-password", CommandLineArgument.ArgumentType.Password, Strings.S3Backend.AuthPasswordDescriptionShort, Strings.S3Backend.AuthPasswordDescriptionLong),
                    new CommandLineArgument("auth-username", CommandLineArgument.ArgumentType.String, Strings.S3Backend.AuthUsernameDescriptionShort, Strings.S3Backend.AuthUsernameDescriptionLong),
                    new CommandLineArgument(EMAIL_OPTION, CommandLineArgument.ArgumentType.String, Strings.S3Backend.AuthEmailDescriptionShort, Strings.S3Backend.AuthEmailDescriptionLong),
                
                });

            }
        }

        public string Description
        {
            get
            {
                return Strings.S3Backend.Description_v2;
            }
        }

        public void Test()
        {
            List();
        }

        public void CreateFolder()
        {
            //S3 does not complain if the bucket already exists
            Connection.AddBucket(m_bucket);
        }

        #endregion

        #region IRenameEnabledBackend Members

        public void Rename(string source, string target)
        {
            Connection.RenameFile(m_bucket, GetFullKey(source), GetFullKey(target));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_options != null)
                m_options = null;
            if (m_wrapper != null)
            {
                m_wrapper.Dispose();
                m_wrapper = null;
            }
        }

        #endregion

        private S3Wrapper Connection
        {
            get { return m_wrapper; }
        }

        private string GetFullKey(string name)
        {
            //AWS SDK encodes the filenames correctly
            return m_prefix + name;
        }
    }
}
