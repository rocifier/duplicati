﻿//  Copyright (C) 2014, Kenneth Skovhede

//  http://www.hexad.dk, opensource@hexad.dk
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System;
using System.Linq;
using System.Collections.Generic;

namespace Duplicati.Library.Main.Operation
{
    internal class ListAffected
    {
        private Options m_options;
        private ListAffectedResults m_result;

        public ListAffected(Options options, ListAffectedResults result)
        {
            m_options = options;
            m_result = result;
        }
            
        public void Run(List<string> args)
        {
            using(var db = new Database.LocalListAffectedDatabase(m_options.Dbpath))
            {
                m_result.SetDatabase(db);
                m_result.SetResult(
                    db.GetFilesets(args).OrderByDescending(x => x.Time).ToArray(),
                    db.GetFiles(args).ToArray(),
                    db.GetLogLines(args).ToArray(),
                    db.GetVolumes(args).ToArray()
                );
            }
        }
    }
}

