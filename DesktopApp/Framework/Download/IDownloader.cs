namespace Framework.Download
{
	public delegate void DownloadProcessEventHandler(object sender, DownloadProcessEventArgs e);

	public delegate void DownloadStartedEventHandler(object sender, DownloadEventArgs e);

	public delegate void DownloadPausedEventHandler(object sender, DownloadEventArgs e);

	public delegate void DownloadCanceledEventHandler(object sender, DownloadEventArgs e);

	public delegate void DownloadComplateEventHandler(object sender, DownloadComplateEventArgs e);

	public delegate void DownloadErrorEventHandler(object sender, DownloadEventArgs e);

	public delegate void DownloadFileErrorEventHandler(object sender, DownloadEventArgs e);

	public interface IDownloader
	{
		string FileUrl { get; set; }

		string FilePath { get; set; }

		long DownId { get; set; }

		bool IsOver { get; set; }

		bool UseMirrorDown { get; set; }

		event DownloadProcessEventHandler DownloadProcess;

		event DownloadStartedEventHandler DownloadStarted;

		event DownloadPausedEventHandler DownloadPaused;

		event DownloadCanceledEventHandler DownloadCanceled;

		event DownloadComplateEventHandler DownloadComplate;

		event DownloadErrorEventHandler DownloadError;

		void Start();

		void Cancel();

		void Stop();
	}
}
