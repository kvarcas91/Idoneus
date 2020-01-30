using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    public class Comment : IComment, INotifyPropertyChanged
    {
        [Key]
        public long ID { get; set; }
        public string Content { get; set; }
        public DateTime SubmitionDate { get; set; }

        public Comment()
        {
            SubmitionDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"ID: {ID}; Submited: {SubmitionDate.ToString()}; Content: {Content}";
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
