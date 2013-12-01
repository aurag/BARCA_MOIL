using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System;
using Npgsql;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Barcelone___OGTS.ViewModel
{
    public class PlanningViewModel : BaseViewModel
    {
        #region Commandes
        public ICommand BackCommand { get; set; }
        public ICommand Export { get; set; }
        public ICommand SwitchPlanningCommand { get; set; }
        #endregion

        #region Properties
        private ICollectionView _days;

        public ICollectionView Days 
        { 
            get
            {
                return _days;
            }
            private set
            {
                _days = value;
                OnPropertyChanged("Days");
            } 
        }
        public string[,] daysPlanning { get; private set; }
        private string _switchPlanning;
        private string _planningTitle;

        public string PlanningTitle
        {
            get { return _planningTitle; }
            set
            {
                _planningTitle = value;
                OnPropertyChanged("PlanningTitle");
            }
        }

        public string SwitchPlanning
        {
            get { return _switchPlanning; }
            set
            {
                _switchPlanning = value;
                OnPropertyChanged("SwitchPlanning");
            }
        }
        #endregion

        /// <summary>
        /// constructeur
        /// </summary>
        public PlanningViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            Export = new Command(param => ExportPlanning(), param => true);
            SwitchPlanningCommand = new Command(param => SwitchPlanningFunction(), param => true);
            SwitchPlanning = "Basculer vers le planning d'équipe";
            PlanningTitle = "Planning personnel";
            CreateDaysPlanningList(false);
        }

        #region Command Methods

        /// <summary>
        /// réponse à la commande back
        /// </summary>
        private void Back()
        {
            Switcher.SwitchBack();
        }
        private void PushChangePassword()
        {
            Switcher.Switch(new ChangePassword());
        }

        private void PushAddInCET()
        {
            Switcher.Switch(new AddInCET());
        }

        private void PushDailyOverview()
        {
            Switcher.Switch(new DailyOverview());
        }


        private void SwitchPlanningFunction()
        {
            if (PlanningTitle.Equals("Planning personnel"))
            {
                PlanningTitle = "Planning d'équipe";
                SwitchPlanning = "Basculer vers le planning personnel";
                CreateDaysPlanningList(true);
            }
            else
            {
                PlanningTitle = "Planning personnel";
                SwitchPlanning = "Basculer vers le planning d'équipe";
                CreateDaysPlanningList(false);
            }
        }

        #endregion

        #region Display Methods

        private void BuildPlanning(Boolean isTeamPlanning)
        {
            // Used to store every day in a year : _daysTmp[month, day] where 0 = january for months
            string[,] daysTmp = new string[12, 31];
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 31; j++)
                {
                    // February with 28 days
                    if (i == 1 && (j == 30 || j == 29 || j == 28))
                        daysTmp[i, j] = "X";
                    else
                    {
                        // Months with 30 days
                        if (((i == 3) || i == 5 || i == 8 || i == 10) && j == 30)
                            daysTmp[i, j] = "X";
                        else
                            daysTmp[i, j] = "";
                    }
                }
            }

            DbHandler.Instance.OpenConnection();
            NpgsqlDataReader result;
            string id_employee = UserSession.Instance.User.Employee.EmployeeId;

            if (isTeamPlanning)
            {
                result = DbHandler.Instance.ExecSQL(@"select id_employee, id_employee_rh from public.employee
                                                                   WHERE public.employee.id_employee_rh=" + id_employee + ";");
                if (result != null)
                {
                    while (result.Read())
                        id_employee += " OR public.dayoff.id_employee=" + result[0].ToString();
                }
                DbHandler.Instance.CloseConnection();
                DbHandler.Instance.OpenConnection();
                result = null;
            }


            result = DbHandler.Instance.ExecSQL(@"select start_date, end_date, type, firstname, lastname from public.dayofftype, public.dayoff INNER JOIN public.employee ON (public.dayoff.id_employee=public.employee.id_employee)
                                                                   WHERE public.dayoff.id_day_off_type = public.dayofftype.id_day_off_type
                                                                   AND (public.dayoff.id_employee=" + id_employee + ");");
            if (result != null)
            {
                while (result.Read())
                {
                    DateTime dayOffStartDate = DateTime.Parse(result[0].ToString().Substring(0, 10));
                    DateTime dayOffEndDate = DateTime.Parse(result[1].ToString().Substring(0, 10));
                    string label = result[2].ToString();

                    // TODO : Add missing types
                    if (label.Equals("01"))
                        label = "CP";
                    if (label.Equals("02"))
                        label = "CA";
                    if (label.Equals("03"))
                        label = "CS";
                    if (label.Equals("04"))
                        label = "RF";

                    if (isTeamPlanning)
                    {
                        // Ajout du trigramme ISO de l'employé à l'origine du congé
                        label += " " + result[3].ToString().Substring(0, 1).ToUpper() + result[4].ToString().Substring(0, 1).ToUpper() + result[4].ToString().Substring(result[4].ToString().Length - 1, 1).ToUpper();
                    }

                    while (dayOffStartDate <= dayOffEndDate)
                    {
                        string labelTmp = daysTmp[dayOffStartDate.Month - 1, dayOffStartDate.Day - 1];
                        if (labelTmp != "" && labelTmp != "X")
                            daysTmp[dayOffStartDate.Month - 1, dayOffStartDate.Day - 1] = labelTmp + " & " + label;
                        else
                            daysTmp[dayOffStartDate.Month - 1, dayOffStartDate.Day - 1] = label;
                        dayOffStartDate = dayOffStartDate.AddDays(1);
                    }
                }
            }
            DbHandler.Instance.CloseConnection();


            DateTime startDate = DateTime.Parse("01/01/2013");
            DateTime endDate = DateTime.Parse("31/12/2013");

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (!daysTmp[date.Month - 1, date.Day - 1].Equals("X"))
                        daysTmp[date.Month - 1, date.Day - 1] = "RE";
                }
            }

            daysPlanning = daysTmp;
        }

        /// <summary>
        /// Create the list of days used to represent the planning
        /// </summary>
        private void CreateDaysPlanningList(Boolean isTeamPlanning)
        {
            List<DayPlanning> _days = new List<DayPlanning>();

            BuildPlanning(isTeamPlanning);
            for (int day = 0; day < 31; day++)
            {

                _days.Add(new DayPlanning(false, daysPlanning[0, day], daysPlanning[1, day], daysPlanning[2, day],
                    daysPlanning[3, day], daysPlanning[4, day], daysPlanning[5, day], daysPlanning[6, day], daysPlanning[7, day],
                    daysPlanning[8, day], daysPlanning[9, day], daysPlanning[10, day], daysPlanning[11, day], day + 1));
            }

            Days = CollectionViewSource.GetDefaultView(_days);
        }


        #endregion

        #region Export Methods
        private void ExportPlanning()
        {
            Microsoft.Office.Interop.Excel.Application oXL;
            Microsoft.Office.Interop.Excel._Workbook oWB;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;

            try
            {
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;
                Microsoft.Office.Interop.Excel.Range oResizeRange;
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

                // Add table headers
                oSheet.Cells[1, 2] = "Janvier 2013";
                oSheet.Cells[1, 3] = "Février 2013";
                oSheet.Cells[1, 4] = "Mars 2013";
                oSheet.Cells[1, 5] = "Avril 2013";
                oSheet.Cells[1, 6] = "Mai 2013";
                oSheet.Cells[1, 7] = "Juin 2013";
                oSheet.Cells[1, 8] = "Juillet 2013";
                oSheet.Cells[1, 9] = "Aout 2013";
                oSheet.Cells[1, 10] = "Septembre 2013";
                oSheet.Cells[1, 11] = "Octobre 2013";
                oSheet.Cells[1, 12] = "Novembre 2013";
                oSheet.Cells[1, 13] = "Décembre 2013";

                // Hearders formating
                oResizeRange = oSheet.get_Range("B1", "M1").get_Resize(Missing.Value);
                oResizeRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
                oResizeRange.EntireColumn.AutoFit();

                for (int mon = 0; mon < 12; mon++)
                {
                    for (int day = 0; day < 31; day++)
                    {
                        // Number of days
                        if (mon == 0)
                            oSheet.Cells[day + 2, 1] = day + 1;

                        // Be careful : for Excel tab[line, col] but for us days[mon, day] !
                        if (daysPlanning[mon, day].Equals(""))
                        {
                            Microsoft.Office.Interop.Excel.Range oRng = GetRange(day, mon, oSheet);
                            oRng.Interior.ColorIndex = 43;
                        }
                        if (daysPlanning[mon, day].Equals("RE"))
                        {
                            oSheet.Cells[day + 2, mon + 2] = "RE";
                            Microsoft.Office.Interop.Excel.Range oRng = GetRange(day, mon, oSheet);
                            oRng.Interior.ColorIndex = 16;
                        }
                        if (daysPlanning[mon, day].Equals("CP"))
                        {
                            oSheet.Cells[day + 2, mon + 2] = "CP";
                            Microsoft.Office.Interop.Excel.Range oRng = GetRange(day, mon, oSheet);
                            oRng.Interior.ColorIndex = 3;
                        }
                        if (daysPlanning[mon, day].Equals("C5"))
                        {
                            oSheet.Cells[day + 2, mon + 2] = "C5";
                            Microsoft.Office.Interop.Excel.Range oRng = GetRange(day, mon, oSheet);
                            oRng.Interior.ColorIndex = 3;
                        }
                        if (daysPlanning[mon, day].Equals("CS"))
                        {
                            oSheet.Cells[day + 2, mon + 2] = "CS";
                            Microsoft.Office.Interop.Excel.Range oRng = GetRange(day, mon, oSheet);
                            oRng.Interior.ColorIndex = 3;
                        }
                        if (daysPlanning[mon, day].Equals("CA"))
                        {
                            oSheet.Cells[day + 2, mon + 2] = "CA";
                            Microsoft.Office.Interop.Excel.Range oRng = GetRange(day, mon, oSheet);
                            oRng.Interior.ColorIndex = 3;
                        }
                        if (daysPlanning[mon, day].Equals("RF"))
                        {
                            oSheet.Cells[day + 2, mon + 2] = "RF";
                            Microsoft.Office.Interop.Excel.Range oRng = GetRange(day, mon, oSheet);
                            oRng.Interior.ColorIndex = 3;
                        }
                        if (daysPlanning[mon, day].Equals("SS"))
                        {
                            oSheet.Cells[day + 2, mon + 2] = "SS";
                            Microsoft.Office.Interop.Excel.Range oRng = GetRange(day, mon, oSheet);
                            oRng.Interior.ColorIndex = 3;
                        }
                    }
                }
                // Borders
                oResizeRange = oSheet.get_Range("B2", "M32").get_Resize(Missing.Value);
                oResizeRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private Microsoft.Office.Interop.Excel.Range GetRange(int day, int mon, Microsoft.Office.Interop.Excel._Worksheet oSheet)
        {
            string tmp = "B";
            switch (mon)
            {
                case 0:
                    tmp = "B";
                    break;
                case 1:
                    tmp = "C";
                    break;
                case 2:
                    tmp = "D";
                    break;
                case 3:
                    tmp = "E";
                    break;
                case 4:
                    tmp = "F";
                    break;
                case 5:
                    tmp = "G";
                    break;
                case 6:
                    tmp = "H";
                    break;
                case 7:
                    tmp = "I";
                    break;
                case 8:
                    tmp = "J";
                    break;
                case 9:
                    tmp = "K";
                    break;
                case 10:
                    tmp = "L";
                    break;
                case 11:
                    tmp = "M";
                    break;
                default:
                    Console.WriteLine("Error month is > 12");
                    break;
            }
            tmp = tmp + (day + 2).ToString();

            Microsoft.Office.Interop.Excel.Range oRng = oSheet.get_Range(tmp, tmp);
            return oRng;
        }
        #endregion


    }
}
