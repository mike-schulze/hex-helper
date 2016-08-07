using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace HexHelper.Libs.Service
{
    /// <summary>
    /// Default implementation for local server that is listening for API messages
    /// </summary>
    public sealed class ServerService : IServerService
    {
        public void Start( int aPort )
        {
            if( !HttpListener.IsSupported )
            {
                OnErrorOccurred( "HttpListener not supported." );
                return;
            }

            if( mListener != null && mListener.IsListening )
            {
                OnErrorOccurred( "Server has already been started." );
                return;
            }

            try
            {
                mListener = new HttpListener();
                mListener.Prefixes.Add( String.Format( "http://localhost:{0}/server/", aPort ) );

                mListener.Start();
                Debug.WriteLine( String.Format( "Listening on port {0}...", aPort ) );

                mListener.BeginGetContext( HandleRequest, mListener );
            }
            catch( HttpListenerException e )
            {
                OnErrorOccurred( e.Message );
            }
        }

        private void HandleRequest( IAsyncResult aResult )
        {
            var theListener = ( HttpListener ) aResult.AsyncState;

            if( theListener == null || !theListener.IsListening )
            {
                return;
            }

            try
            {
                var theContext = theListener.EndGetContext( aResult );
                var theRequest = theContext.Request;

                if( theRequest.InputStream != null )
                {
                    using( var theReader = new StreamReader( theRequest.InputStream ) )
                    {
                        var theContent = theReader.ReadToEnd();
                        if( !String.IsNullOrWhiteSpace( theContent ) )
                        {
                            OnDataPosted( theContent );
                        }
                    }
                }

                var theResponse = theContext.Response;
                string theResponseString = "<HTML><BODY>Hello world!</BODY></HTML>";
                byte[] theBuffer = Encoding.UTF8.GetBytes( theResponseString );

                theResponse.ContentLength64 = theBuffer.Length;
                var theOutput = theResponse.OutputStream;
                theOutput.Write( theBuffer, 0, theBuffer.Length );
                theOutput.Close();

                if( theListener.IsListening )
                {
                    mListener.BeginGetContext( HandleRequest, mListener );
                }
            }
            catch( HttpListenerException e )
            {
                OnErrorOccurred( e.Message );
            }
        }

        public void Stop()
        {
            if( mListener != null && mListener.IsListening )
            {
                try
                {
                    mListener.Stop();
                    mListener = null;
                }
                catch( HttpListenerException e )
                {
                    OnErrorOccurred( e.Message );
                }
            }
        }

        private void OnDataPosted( string aData )
        {
            if( DataPosted != null )
            {
                DataPosted( this, aData );
            }
        }
        public event EventHandler<string> DataPosted;

        private void OnErrorOccurred( string aData )
        {
            if( ErrorOccurred != null )
            {
                ErrorOccurred( this, aData );
            }
        }
        public event EventHandler<string> ErrorOccurred;

        private HttpListener mListener;
    }
}
