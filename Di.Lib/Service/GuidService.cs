using Microsoft.Extensions.Logging;
using System;

namespace Di.Lib.Service
{
    public class GuidService : IGuidService
    {
        private readonly ILogger _log;
        public GuidService(ILoggerFactory logFactory)
        {
            if (logFactory == null) throw new ArgumentNullException(nameof(logFactory));
            _log = logFactory.CreateLogger<GuidService>();
            _log.LogTrace("Trace: GuidService created");
        }

        public Guid NewGuid()
        {
            var guid = Guid.NewGuid();
            _log.LogTrace("Trace: New GUID {guid}", guid);
            return guid;
        }
    }
}
