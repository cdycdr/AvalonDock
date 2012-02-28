using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;

namespace AvalonDock.MVVMTestApp
{
    class FileViewModel : ViewModelBase
    {
        public FileViewModel(string filePath)
        {
            FilePath = filePath;
        }

        public FileViewModel()
        {
            IsDirty = true;
        }

        #region FilePath
        private string _filePath = null;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    RaisePropertyChanged("FilePath");
                    RaisePropertyChanged("FileName");

                    if (File.Exists(_filePath))
                        _textContent = File.ReadAllText(_filePath);
                }
            }
        }
        #endregion


        public string FileName
        {
            get 
            {
                if (FilePath == null)
                    return "Noname" + (IsDirty ? "*" : "");

                return System.IO.Path.GetFileName(FilePath) + (IsDirty ? "*" : ""); 
            }
        }


        #region TextContent

        private string _textContent = string.Empty;
        public string TextContent
        {
            get { return _textContent; }
            set
            {
                if (_textContent != value)
                {
                    _textContent = value;
                    RaisePropertyChanged("TextContent");
                    IsDirty = true;
                }
            }
        }

        #endregion

        #region IsDirty

        private bool _isDirty = false;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    RaisePropertyChanged("IsDirty");
                    RaisePropertyChanged("FileName");
                }
            }
        }

        #endregion

    }
}
