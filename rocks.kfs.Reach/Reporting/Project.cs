// <copyright>
// Copyright 2019 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;

namespace rocks.kfs.Reach.Reporting
{
    public class Project
    {
        public int? id;
        public int? account_id;
        public string cover_image_content_type;
        public string cover_image_file_name;
        public int? cover_image_file_size;
        public DateTime? cover_image_updated_at;
        public CoverImages cover_images;
        public DateTime? created_at;
        public string description;
        public bool disable;
        public string full_url;
        public string get_involved;
        public string image_content_type;
        public string image_file_name;
        public int? image_file_size;
        public DateTime? image_updated_at;
        public ProjectImages images;
        public string language;
        public string leader;
        public string leader_email;
        public string leader_phone;
        public string permalink;
        public string subtitle;
        public string title;
        public DateTime? updated_at;
        public string url;
        public string web_address;

    }
    public class ProjectImages
    {
        public string thumbnail;
        public string small;
        public string medium;
        public string large;
        public string original;
    }

    public class CoverImages
    {
        public string cover;
        public string original;
    }
}
