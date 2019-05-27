using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FChan.Library
{
    /// <summary>
    ///     Chan.
    /// </summary>
    public static class Chan
    {
        private static async Task<string> GetStringAsync(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var task = new TaskCompletionSource<WebResponse>();

            _ = request.BeginGetResponse(ac =>
              {
                  try
                  {
                      task.SetResult(request.EndGetResponse(ac));
                  }
                  catch (Exception e)
                  {
                      task.SetException(e);
                  }
              }, null);

            using (WebResponse response = await task.Task)
            using (Stream stream = response.GetResponseStream())
            using (var output = new MemoryStream())
            {
                await stream.CopyToAsync(output);
                byte[] array = output.ToArray();
                return Encoding.UTF8.GetString(array, 0, array.Length);
            }
        }

        internal static T DownloadObject<T>(string url)
        {
            Task<T> task = DownloadObjectAsync<T>(url);
            task.Wait();
            return task.Result;
        }

        internal static async Task<T> DownloadObjectAsync<T>(string url)
        {
            try
            {
                string response = await GetStringAsync(url);
                string responseString = response;
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Gets boards info.
        /// </summary>
        /// <returns>The board.</returns>
        public static BoardRootObject GetBoard() => DownloadObject<BoardRootObject>(Constants.BoardUrl);

        /// <summary>
        ///     Gets boards info asynchronously.
        /// </summary>
        /// <returns>The board.</returns>
        public static async Task<BoardRootObject> GetBoardAsync() => await DownloadObjectAsync<BoardRootObject>(Constants.BoardUrl);

        /// <summary>
        ///     Gets the thread.
        /// </summary>
        /// <returns>The thread.</returns>
        /// <param name="board">Board.</param>
        /// <param name="threadNumber">Thread number.</param>
        public static Thread GetThread(string board, int threadNumber)
        {
            Thread thread = DownloadObject<Thread>(Constants.GetThreadUrl(board, threadNumber));
            foreach (Post item in thread.Posts)
            {
                item.Board = board;
            }

            return thread;
        }

        /// <summary>
        ///     Gets the thread asynchronously.
        /// </summary>
        /// <returns>The thread.</returns>
        /// <param name="board">Boad.</param>
        /// <param name="threadNumber">Thread number.</param>
        public static async Task<Thread> GetThreadAsync(string board, int threadNumber)
        {
            Thread thread = await DownloadObjectAsync<Thread>(Constants.GetThreadUrl(board, threadNumber));
            foreach (Post item in thread.Posts)
            {
                item.Board = board;
            }

            return thread;
        }

        /// <summary>
        ///     Gets thead root object.
        /// </summary>
        /// <returns>The thread page.</returns>
        /// <param name="board">Board.</param>
        /// <param name="page">Page.</param>
        public static ThreadRootObject GetThreadPage(string board, int page)
        {
            ThreadRootObject thread = DownloadObject<ThreadRootObject>(Constants.GetThreadPageUrl(board, page));

            foreach (Thread item in thread.Threads)
            {
                foreach (Post post in item.Posts)
                {
                    post.Board = board;
                }
            }

            return thread;
        }

        /// <summary>
        ///     Gets thead root object asynchronously.
        /// </summary>
        /// <returns>The thread page.</returns>
        /// <param name="board">Board.</param>
        /// <param name="page">Page.</param>
        public static async Task<ThreadRootObject> GetThreadPageAsync(string board, int page)
        {
            ThreadRootObject thread = await DownloadObjectAsync<ThreadRootObject>(Constants.GetThreadPageUrl(board, page));

            foreach (Thread item in thread.Threads)
            {
                foreach (Post post in item.Posts)
                {
                    post.Board = board;
                }
            }

            return thread;
        }
    }
}