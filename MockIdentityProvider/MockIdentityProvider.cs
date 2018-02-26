using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace MockIdentityProvider
{
    public class MockIdentityProvider
    {
        private readonly HttpListener httpListener;
        private readonly IDictionary<string, Action<HttpListenerRequest, HttpListenerResponse>> routeHandlers =
            new Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse>>();

        private readonly Routes routes;

        public MockIdentityProvider() : this(new Routes()) { }

        public MockIdentityProvider(Routes routes)
        {
            this.routes = routes;

            httpListener = new HttpListener();

            httpListener.Prefixes.Add("http://localhost:2345/");

            routeHandlers.Add(routes.AuthorizeRoute, HandleAuthorizeRequest);
            routeHandlers.Add(routes.TokenRoute, HandleTokenRequest);
            routeHandlers.Add(routes.UserInfoRoute, HandleTokenRequest);
        }

        public void Start()
        {
            httpListener.Start();

            ThreadPool.QueueUserWorkItem(o =>
            {
                Console.WriteLine("Listener running...");
                try
                {
                    while (httpListener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(c =>
                        {
                            var ctx = c as HttpListenerContext;

                            try
                            {
                                ctx.Response.ContentType = "application/json";

                                GenerateResponse(ctx.Request, ctx.Response);
                                //byte[] buf = Encoding.UTF8.GetBytes(response);
                                //ctx.Response.ContentLength64 = buf.Length;
                                //ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, httpListener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        private void GenerateResponse(
            HttpListenerRequest request,
            HttpListenerResponse response)
        {
            var path = request.Url.AbsolutePath;

            if (!routeHandlers.TryGetValue(path, out Action<HttpListenerRequest, HttpListenerResponse> handler))
            {
                throw new RouteHandlerNotAssignedException();
            }

            handler.Invoke(request, response);
        }

        private void HandleAuthorizeRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var eventArgs = new GenerateResponseArgs { Request = request, Response = response };

            OnGenerateAuthorizeResponse?.Invoke(this, eventArgs);

            response.StatusCode = (int)HttpStatusCode.OK;
        }

        private void HandleTokenRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var eventArgs = new GenerateResponseArgs { Request = request, Response = response };

            OnGenerateTokenResponse?.Invoke(this, eventArgs);

            response.StatusCode = (int)HttpStatusCode.PaymentRequired;
        }

        private void HandleUserInfoRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var eventArgs = new GenerateResponseArgs { Request = request, Response = response };

            OnGenerateUserInfoResponse?.Invoke(this, eventArgs);

            response.StatusCode = (int)HttpStatusCode.Unused;
        }

        public void Stop()
        {
            httpListener.Stop();
        }

        public event EventHandler<GenerateResponseArgs> OnGenerateAuthorizeResponse;
        public event EventHandler<GenerateResponseArgs> OnGenerateTokenResponse;
        public event EventHandler<GenerateResponseArgs> OnGenerateUserInfoResponse;
    }
}
