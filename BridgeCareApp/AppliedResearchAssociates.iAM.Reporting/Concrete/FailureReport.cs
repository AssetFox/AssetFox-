﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common.Logging;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class FailureReport : IReport
    {
        private Guid _id;
        private List<string> _errorList;
        private bool _isComplete;

        public FailureReport()
        {
            _id = Guid.NewGuid();
            _errorList = new List<string>();
            _isComplete = false;
        }

        public Guid ID {
            get => _id;
            set { _id = value; }
        }

        public Guid? SimulationID { get => null; set { } }

        public string Results { get => String.Empty; set { } }

        public ReportType Type => ReportType.HTML;

        public string ReportTypeName => "Failure Report";

        public List<string> Errors => _errorList;

        public bool IsComplete => _isComplete;

        public string Status => String.Empty;

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            _isComplete = false;
            string errorMessage = parameters;
            _errorList.Add(errorMessage);
            _isComplete = true;
        }
    }
}
