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
        /// Returns the link of a picture having the closest width.
        /// </summary>
        /// <param name="client">The VimeoClient to use.</param>
        /// <param name="videoId">The Vimeo Id of the video.</param>
        /// <param name="width">The desired width of the picture.</param>
        public string GetPicture( VimeoClient client, long videoId, int width = 1920 )
        {
            var link = string.Empty;

            var pictureList = new List<VimeoDotNet.Models.Picture>();
            var task = Task.Run( async () =>
            {
                var pictures = await client.GetPicturesAsync( videoId );
                pictureList = pictures.data.ToList();
            } );
            task.Wait();
            if ( pictureList.Count > 0 )
            {
                var picSizes = new List<VimeoDotNet.Models.Size>();
                picSizes = pictureList.FirstOrDefault().sizes.ToList();
                link = picSizes.Aggregate( ( x, y ) => Math.Abs( x.width - width ) < Math.Abs( y.width - width ) ? x : y ).link;
            }
            return link;
        }

        /// <summary>
        /// Returns a VideoInfo object with video info.
        /// </summary>
        /// <param name="client">The VimeoClient to use.</param>
        /// <param name="videoId">The Vimeo Id of the video.</param>
        public VideoInfo GetVideoInfo( VimeoClient client, long videoId )
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
            public int duration;
            public string hdLink;
            public string sdLink;
            public string hlsLink;
        }
    }
}
