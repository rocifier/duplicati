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
using Amazon.S3;
using Amazon.S3.Model;
using System.Xml;
using Duplicati.Library.Interface;

namespace Duplicati.Library.Backend
{
    /// <summary>
    /// Helper class that fixes long list support and injects location headers, includes using directives etc.
    /// </summary>
    public class S3Wrapper : IDisposable
    {
        private const int ITEM_LIST_LIMIT = 1000;

        protected string m_locationConstraint;
        protected bool m_useRRS;
		protected AmazonS3Client m_client;

        protected Amazon.RegionEndpoint GetRegionFromStringIdentifier(string region)
        {
            switch (region)
            {
                case "eu-west-1":
                    return Amazon.RegionEndpoint.EUWest1;
                case "eu-central-1":
                    return Amazon.RegionEndpoint.EUCentral1;
                case "us-east-1":
                    return Amazon.RegionEndpoint.USEast1;
                case "us-west-1":
                    return Amazon.RegionEndpoint.USWest1;
                case "us-west-2":
                    return Amazon.RegionEndpoint.USWest2;
                case "ap-southeast-1":
                    return Amazon.RegionEndpoint.APSoutheast1;
                case "ap-southeast-2":
                    return Amazon.RegionEndpoint.APSoutheast2;
                case "ap-northeast-1":
                    return Amazon.RegionEndpoint.APNortheast1;
                case "sa-east-1":
                    return Amazon.RegionEndpoint.SAEast1;
                default:
                    return Amazon.RegionEndpoint.USEast1;
            }
        }

        public S3Wrapper(string awsID, string awsKey, string locationConstraint)
        {
            AmazonS3Config cfg = new AmazonS3Config()
            {
                UserAgent = "CBD Backup v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " S3 client with AWS SDK v" + GetType().Assembly.GetName().Version.ToString(),
                AuthenticationRegion = locationConstraint,
                RegionEndpoint = GetRegionFromStringIdentifier(locationConstraint),
                BufferSize = (int)Duplicati.Library.Utility.Utility.DEFAULT_BUFFER_SIZE
            };

            m_client = new Amazon.S3.AmazonS3Client(awsID, awsKey, cfg);

            m_locationConstraint = locationConstraint;
            m_useRRS = true; //useRRS;
        }

        public void AddBucket(string bucketName)
        {
            PutBucketRequest request = new PutBucketRequest();
            request.BucketName = bucketName;

            if (!string.IsNullOrEmpty(m_locationConstraint))
                request.BucketRegionName = m_locationConstraint;

            PutBucketResponse response = m_client.PutBucket(request);
        }

        public virtual void GetFileStream(string bucketName, string keyName, System.IO.Stream target)
        {
            GetObjectRequest objectGetRequest = new GetObjectRequest() {
                BucketName = bucketName,
                Key = keyName
            };

            using (GetObjectResponse objectGetResponse = m_client.GetObject(objectGetRequest))
            {
                using (System.IO.Stream s = objectGetResponse.ResponseStream)
                {
                    try { s.ReadTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds; }
                    catch { }

                    Utility.Utility.CopyStream(s, target);
                }
            }
        }

        public void GetFileObject(string bucketName, string keyName, string localfile)
        {
            using (System.IO.FileStream fs = System.IO.File.Open(localfile, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                GetFileStream(bucketName, keyName, fs);
        }

        public void AddFileObject(string bucketName, string keyName, string localfile)
        {
            using (System.IO.FileStream fs = System.IO.File.Open(localfile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                AddFileStream(bucketName, keyName, fs);
        }

        public virtual void AddFileStream(string bucketName, string keyName, System.IO.Stream source)
        {
            // todo: md5 digest without reading stream twice
            PutObjectRequest objectAddRequest = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = source,
                StorageClass = m_useRRS ? S3StorageClass.ReducedRedundancy : S3StorageClass.Standard
            };
             
            PutObjectResponse objectAddResponse = m_client.PutObject(objectAddRequest);
        }

        public void DeleteObject(string bucketName, string keyName)
        {
            DeleteObjectRequest objectDeleteRequest = new DeleteObjectRequest()
            {
                BucketName = bucketName,
                Key = keyName
            };

            DeleteObjectResponse objectDeleteResponse = m_client.DeleteObject(objectDeleteRequest);
        }

        public virtual List<IFileEntry> ListBucket(string bucketName, string prefix)
        {
            bool isTruncated = true;
            string filename = null;

            List<IFileEntry> files = new List<IFileEntry>();

            //We truncate after ITEM_LIST_LIMIT elements, and then repeat
            while (isTruncated)
            {
                ListObjectsRequest listRequest = new ListObjectsRequest();
                listRequest.BucketName = bucketName;

                if (!string.IsNullOrEmpty(filename))
                    listRequest.Marker = filename;

                listRequest.MaxKeys = ITEM_LIST_LIMIT;
                if (!string.IsNullOrEmpty(prefix))
                    listRequest.Prefix = prefix;

                ListObjectsResponse listResponse = m_client.ListObjects(listRequest);

                isTruncated = listResponse.IsTruncated;
                filename = listResponse.NextMarker;

                foreach (S3Object obj in listResponse.S3Objects)
                {
                    files.Add(new FileEntry(
                        obj.Key,
                        obj.Size,
                        obj.LastModified,
                        obj.LastModified
                    ));
                }

                //filename = files[files.Count - 1].Name;
            }

            //TODO: Figure out if this is the case with AWSSDK too
            //Unfortunately S3 sometimes reports duplicate values when requesting more than one page of results
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            for (int i = 0; i < files.Count; i++)
                if (tmp.ContainsKey(files[i].Name))
                {
                    files.RemoveAt(i);
                    i--;
                }
                else
                    tmp.Add(files[i].Name, null);

            return files;
        }

        public void RenameFile(string bucketName, string source, string target)
        {
            CopyObjectRequest copyObjectRequest = new CopyObjectRequest()
            {
                SourceBucket = bucketName,
                SourceKey = source,
                DestinationBucket = bucketName,
                DestinationKey = target
            };

            CopyObjectResponse copyObjectResponse = m_client.CopyObject(copyObjectRequest);

            DeleteObject(bucketName, source);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_client != null)
                m_client.Dispose();
            m_client = null;
        }

        #endregion
    }
}
