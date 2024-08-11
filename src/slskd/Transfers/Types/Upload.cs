﻿// <copyright file="Upload.cs" company="slskd Team">
//     Copyright (c) slskd Team. All rights reserved.
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as published
//     by the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Affero General Public License for more details.
//
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see https://www.gnu.org/licenses/.
// </copyright>

namespace slskd.Transfers
{
    using System;
    using System.Threading.Tasks;

    public sealed class Upload
    {
        public DateTime Enqueued { get; set; } = DateTime.UtcNow;
        public string Filename { get; set; }
        public string Group { get; set; }
        public DateTime? Ready { get; set; } = null;
        public DateTime? Started { get; set; } = null;
        public TaskCompletionSource TaskCompletionSource { get; set; } = new TaskCompletionSource();
        public string Username { get; set; }
    }
}