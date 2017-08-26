using System;
using System.ComponentModel;

namespace LteDev
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public struct ComparisonResult
    {
        private string _description;
        private string _name;
        private string _masterPath;
        private string _targetPath;
        private bool _isFile;
        private string _message;
        private ComparisonStatus _status;

        private static string GetMessage(ComparisonStatus status, bool isFile)
        {
            string message;
            switch (status)
            {
                case ComparisonStatus.MasterNotFound:
                    {
                        message = "{0} not found at master location.";
                        break;
                    }
                case ComparisonStatus.TargetNotFound:
                    {
                        message = "{0} not found at target location.";
                        break;
                    }
                case ComparisonStatus.Changed:
                    {
                        message = "File changed.";
                        break;
                    }
                default:
                    {
                        message = "Success";
                        break;
                    }
            }

            return String.Format(message, (isFile) ? "File" : "Folder");
        }
        public ComparisonResult(string description, string masterPath, string targetPath, ComparisonStatus status)
        {
            _description = description;
            _masterPath = masterPath;
            _targetPath = targetPath;
            _status = status;
            _isFile = false;
            _name = "";
            _message = ComparisonResult.GetMessage(status, false);
        }

        public ComparisonResult(string description, string masterPath, string targetPath, ComparisonStatus status, string name, bool isFile)
        {
            _description = description;
            _masterPath = masterPath;
            _targetPath = targetPath;
            _name = name;
            _status = status;
            _isFile = isFile;
            _message = ComparisonResult.GetMessage(status, isFile);
        }

        public ComparisonStatus Status { get { return this._status; } }

        public string Description { get { return this._description; } }

        public string Name { get { return this._name; } }

        public string MasterPath { get { return this._masterPath; } }

        public string TargetPath { get { return this._targetPath; } }

        public bool IsFile { get { return this._isFile; } }

        public string Message { get { return this._message; } }

        public static BindingList<ComparisonResult> MakeBindingList(params ComparisonResult[] items)
        {
            return new BindingList<ComparisonResult>(items);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}