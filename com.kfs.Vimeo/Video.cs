using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VimeoDotNet;

namespace com.kfs.Vimeo
{
    public class Video
    {
        /// <summary>
        /// Returns a VideoInfo object with video info.
        /// </summary>
        /// <param name="client">The VimeoClient to use.</param>
        /// <param name="videoId">The Vimeo Id of the video.</param>
        public VideoInfo GetVideoInfo( VimeoClient client, long videoId, int width = 1920 )
        {
            var videoInfo = new VideoInfo();

            var task = Task.Run( async () =>
            {
                var video = await client.GetVideoAsync( videoId );
                videoInfo.vimeoId = videoId;
                videoInfo.name = video.name;
                videoInfo.description = string.Format( "<p>{0}</p>", video.description.Replace( "\n", "<br>" ) );
                videoInfo.duration = video.duration;
                videoInfo.hdLink = video.HighDefinitionVideoSecureLink;
                videoInfo.sdLink = video.StandardVideoSecureLink;
                videoInfo.hlsLink = video.StreamingVideoSecureLink;

                
                //
                // for 0.8.x structure
                //
                var pictures = video.pictures.ToList();
                var picSizes = pictures.FirstOrDefault( p => p.active == true ).sizes.ToList();

                //
                // for > 0.8.x vimeo dot net structure
                //
                //var picSizes = video.pictures.sizes.ToList();

                if ( picSizes.Count > 0 )
                {
                    videoInfo.imageUrl = picSizes.Aggregate( ( x, y ) => Math.Abs( x.width - width ) < Math.Abs( y.width - width ) ? x : y ).link;
                }
            } );
            task.Wait();

            return videoInfo;
        }

        /// <summary>
        /// An object to hold the links to the video files.
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
