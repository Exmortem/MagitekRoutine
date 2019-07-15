using System;

namespace Magitek.Models.MagitekApi
{
    public class MagitekNews
    {
        public int Id { get; set; }
        public string Created { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }

        public DateTime DateTime => Convert.ToDateTime(Created);
        public int DayPosted => DateTime.Day;

        public string MonthPosted
        {
            get
            {
                switch (DateTime.Month)
                {
                    case 1:
                        return "JAN";
                    case 2:
                        return "FEB";
                    case 3:
                        return "MAR";
                    case 4:
                        return "APR";
                    case 5:
                        return "MAY";
                    case 6:
                        return "JUNE";
                    case 7:
                        return "JULY";
                    case 8:
                        return "AUG";
                    case 9:
                        return "SEPT";
                    case 10:
                        return "OCT";
                    case 11:
                        return "NOV";
                    case 12:
                        return "DEC";
                }

                return "";
            }
        }
    }
}
