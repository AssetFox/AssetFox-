using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Common.Logging
{
    public class CompoundLog : ILog
    {
        private readonly ILog[] _childLoggers;

        public CompoundLog(params ILog[] childLoggers)
        {
            _childLoggers = childLoggers;
        }
        public void Debug(string message)
        {
            foreach (var child in _childLoggers)
            {
                child.Debug(message);
            }
        }

        public void Error(string message)
        {
            foreach (var child in _childLoggers)
            {
                child.Error(message);
            }
        }

        public void Warning(string message)
        {
            foreach (var child in _childLoggers)
            {
                child.Warning(message);
            }
        }
        public void Information(string message)
        {
            foreach (var child in _childLoggers)
            {
                child.Information(message);
            }
        }
    }
}
