using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Domain.Models.Tasks
{

    [Table("repetetive_tasks")]
    [Serializable]
    public class RepetetiveTask : IEntity
    {

        [Key]
        public string ID { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Dates { get; set; } = "0000000";

        [Computed]
        public ObservableCollection<RepetetiveDay> DateList { get; set; }

        public RepetetiveTask()
        {
            CreateDateList();
        }

        public void SetDayList(bool includeParentID = false)
        {
            if (string.IsNullOrEmpty(Dates))
            {
                CreateDateList();
                return;
            }

            for (int i = 0; i < Dates.Length; i++)
            {
                var c = Dates[i];

                DateList[i].IsActive = c.Equals('0') ? false : true;
                if (includeParentID) DateList[i].ParentID = ID;
            }
        }

        public bool IsActiveToday()
        {
            var weekDay = (int)DateTime.Now.DayOfWeek;
            return DateList[weekDay].IsActive;
        }

        public void SetDate()
        {
            var builder = new StringBuilder();
            foreach (var item in DateList)
            {
                builder.Append(item.IsActive ? "1" : "0");
            }
            Dates = builder.ToString();
        }

        public void CreateDateList ()
        {
            if (DateList == null)
            {
                DateList = new ObservableCollection<RepetetiveDay>
                {
                    new RepetetiveDay() { DayName = "S"},
                    new RepetetiveDay() { DayName = "M"},
                    new RepetetiveDay() { DayName = "T"},
                    new RepetetiveDay() { DayName = "W"},
                    new RepetetiveDay() { DayName = "T"},
                    new RepetetiveDay() { DayName = "F"},
                    new RepetetiveDay() { DayName = "S"},
                };
            }
        }

    }
}
