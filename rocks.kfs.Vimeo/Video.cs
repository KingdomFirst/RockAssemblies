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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VimeoDotNet;

namespace rocks.kfs.Vimeo
{
    public class Video
    {
        /// <summary>
        /// Returns a VideoInfo object with video info.
        /// </summary>
        /// <param name="client">The VimeoClient to use.</param>
        /// <param name="videoId">The Vimeo Id of the video.</param>
        /// <param name="width">The closest width of the image url to return. Default is 1920px.</param>
        public VideoInfo GetVideoInfo( VimeoClient client, long videoId, int width = 1920 )
        {
            var videoInfo = new VideoInfo();

            var task = Task.Run( async () =>
            {
                var video = await client.GetVideoAsync( videoId );
                videoInfo.vimeoId = videoId;
                videoInfo.name = video.Name;
                if ( !string.IsNullOrWhiteSpace( video.Description ) )
                {
                    videoInfo.description = string.Format( "<p>{0}</p>", video.Description.Replace( "\n", "<br>" ) );
                }
                else
                {
                    videoInfo.description = "";
                }
                videoInfo.duration = video.Duration;
                videoInfo.hdLink = video.HighDefinitionVideoSecureLink;
                videoInfo.sdLink = video.StandardVideoSecureLink;
                videoInfo.hlsLink = video.StreamingVideoSecureLink;

                //
                // for 0.8.x vimeo dot net structure
                //
                var pictures = video.Pictures;
                var picSizes = pictures.Sizes.ToList();

                //
                // for > 0.8.x vimeo dot net structure
                //
                //var picSizes = video.pictures.sizes.ToList();

                if ( picSizes.Count > 0 )
                {
                    videoInfo.imageUrl = picSizes.Aggregate( ( x, y ) => Math.Abs( x.Width - width ) < Math.Abs( y.Width - width ) ? x : y ).Link;
                }
            } );
            task.Wait();

            return videoInfo;
        }

        /// <summary>
        /// Returns a List of VideoInfo objects with all video info from a user.
        /// </summary>
        /// <param name="client">The VimeoClient to use.</param>
        /// <param name="width">The closest width of the image url to return. Default is 1920px.</param>
        public List<VideoInfo> GetVideos( VimeoClient client, long userId, int width = 1920 )
        {
            var retVal = new List<VideoInfo>();
            var videos = new List<VimeoDotNet.Models.Video>();

            int page = 1;

            while ( page > 0 )
            {
                VimeoDotNet.Models.Paginated<VimeoDotNet.Models.Video> pagedVideos = null;
                var task = Task.Run( async () =>
                {
                    pagedVideos = await client.GetVideosAsync( userId, page, null );
                } );
                task.Wait();

                videos.AddRange( pagedVideos.Data );

                int num = 0;

                if ( pagedVideos.Paging.Next != null )
                {
                    int.TryParse( pagedVideos.Paging.Next.Split( '=' ).Last(), out num );
                }

                page = num;
            }

            foreach ( var video in videos )
            {
                var videoInfo = new VideoInfo();

                videoInfo.vimeoId = ( long ) video.Id;
                videoInfo.name = video.Name;
                if ( string.IsNullOrWhiteSpace( video.Description ) )
                {
                    videoInfo.description = string.Empty;
                }
                else
                {
                    videoInfo.description = string.Format( "<p>{0}</p>", video.Description.Replace( "\n", "<br>" ) );
                }
                videoInfo.duration = video.Duration;
                videoInfo.hdLink = video.HighDefinitionVideoSecureLink;
                videoInfo.sdLink = video.StandardVideoSecureLink;
                videoInfo.hlsLink = video.StreamingVideoSecureLink;

                if ( video.Pictures != null )
                {
                    //
                    // for 0.8.x vimeo dot net structure
                    //

                    var pictures = video.Pictures;
                    var picSizes = pictures.Sizes;

                    //
                    // for > 0.8.x vimeo dot net structure
                    //
                    //var picSizes = video.pictures.sizes.ToList();

                    if ( picSizes.Count > 0 )
                    {
                        videoInfo.imageUrl = picSizes.Aggregate( ( x, y ) => Math.Abs( x.Width - width ) < Math.Abs( y.Width - width ) ? x : y ).Link;
                    }
                }

                retVal.Add( videoInfo );
            }

            return retVal;
        }

        /// <summary>
        /// An object to hold the returned data for a Vimeo API call.
        /// </summary>
        public class VideoInfo
        {
            public long vimeoId;
            public string name;
            public string description;
            public string imageUrl;
            public int duration;
            public string hdLink;
            public string sdLink;
            public string hlsLink;
        }
    }
}