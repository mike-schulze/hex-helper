using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HexHelper.WinDesktop.Service
{
    public sealed class ServerService : IServerService
    {
        public void Start( int port )
        {
            if( !HttpListener.IsSupported )
            {
                OnErrorOccurred( "HttpListener not supported." );
                return;
            }

            if( _listener != null && _listener.IsListening )
            {
                OnErrorOccurred( "Server has already been started." );
                return;
            }

            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add( String.Format( "http://localhost:{0}/server/", port ) );

                _listener.Start();
                Debug.WriteLine( String.Format( "Listening on port {0}...", port ) );

                _listener.BeginGetContext( HandleRequest, _listener );
            }
            catch( HttpListenerException e )
            {
                OnErrorOccurred( e.Message );
            }
        }

        private void HandleRequest( IAsyncResult result )
        {
            HttpListener listener = ( HttpListener ) result.AsyncState;

            if( listener == null || !listener.IsListening )
            {
                return;
            }

            try
            {
                var context = listener.EndGetContext( result );
                var request = context.Request;

                if( request.InputStream != null )
                {
                    using( var reader = new StreamReader( request.InputStream ) )
                    {
                        var content = reader.ReadToEnd();
                        if( !String.IsNullOrWhiteSpace( content ) )
                        {
                            OnDataPosted( content );
                        }
                    }
                }

                var response = context.Response;
                string responseString = "<HTML><BODY>Hello world!</BODY></HTML>";
                byte[] buffer = Encoding.UTF8.GetBytes( responseString );

                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;
                output.Write( buffer, 0, buffer.Length );
                output.Close();

                if( listener.IsListening )
                {
                    _listener.BeginGetContext( HandleRequest, _listener );
                }
            }
            catch( HttpListenerException e )
            {
                OnErrorOccurred( e.Message );
            }
        }

        public void Stop()
        {
            if( _listener != null && _listener.IsListening )
            {
                try
                {
                    _listener.Stop();
                    _listener = null;
                }
                catch( HttpListenerException e )
                {
                    OnErrorOccurred( e.Message );
                }
            }
        }

        private void OnDataPosted( string data )
        {
            if( DataPosted != null )
            {
                DataPosted( this, data );
            }
        }
        public event EventHandler<string> DataPosted;

        private void OnErrorOccurred( string data )
        {
            if( ErrorOccurred != null )
            {
                ErrorOccurred( this, data );
            }
        }
        public event EventHandler<string> ErrorOccurred;

        private HttpListener _listener;
    }
}
