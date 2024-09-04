using PowerAPI.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Service.Helper;
using PowerAPI.Data.ViewModels;

namespace PowerAPI.Service.Clients
{
    public class Attendance : IAttendance
    {
        EnterpriseContext _DBContext;
        private readonly IEmployee _employee;

        public Attendance(EnterpriseContext DBContext,IEmployee employee)
        {
            _DBContext = DBContext;
            _employee = employee;
        }
        public async Task<StatusMessage> Add(PayrollHrpayrollAttDetail attendance, string Mode, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == attendance.EmployeeId).FirstOrDefaultAsync();

                var currentAttendance = await _DBContext.PayrollHrpayrollAttDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == attendance.EmployeeId &&
                                                            x.AttendanceDate == DateTime.Now.Date).FirstOrDefaultAsync();

                if (emp != null)
                {
                    if (currentAttendance != null)
                    {
                        if ((currentAttendance.ClockedIn == true || currentAttendance.TimeIn != null) && currentAttendance.ClockedOut == false)
                        {
                            //update clock out
                            currentAttendance.CompanyId = token.CompanyId;
                            currentAttendance.DivisionId = token.DepartmentId;
                            currentAttendance.DepartmentId = token.DepartmentId;
                            currentAttendance.ClockedOut = true;
                            currentAttendance.ClockOutTimeOut = attendance.ClockOutTimeOut == null ? DateTime.Now : attendance.ClockOutTimeOut;
                            currentAttendance.AttendanceStatus = "Completed";
                            currentAttendance.GeolocationAddress2 = attendance.GeolocationAddress2;

                            _DBContext.Entry(currentAttendance).State = EntityState.Modified;
                            _DBContext.SaveChanges();

                            statusMessage.Status = "Success";
                            statusMessage.Message = "Clock Out Successfull";

                        }
                    }
                    else
                    {
                        //update clock in
                        attendance.CompanyId = token.CompanyId;
                        attendance.DivisionId = token.DepartmentId;
                        attendance.DepartmentId = token.DepartmentId;
                        attendance.SystemDate = DateTime.Now;
                        attendance.ClockedIn = true;
                        attendance.ClockedOut = false;
                        attendance.TimeIn = attendance.TimeIn == null ? DateTime.Now : attendance.TimeIn;
                        attendance.ClockOutTimeOut = null;
                        attendance.AttendanceStatus = "Clocked In";
                        attendance.Remarks = attendance.Remarks == "" || attendance.Remarks == null ? "Attendance Done on ESS" : attendance.Remarks;


                        //attendance.CompanyId = token.CompanyId;
                        //attendance.DivisionId = token.DivisionId;
                        //attendance.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(attendance).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Clock In Successfull";

                    }

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Employee Does Not Exist";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public Task<StatusMessage> Delete(PayrollHrpayrollAttDetail attendance, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiToken> GetAccess(string token)
        {
            int days = 0;
            ApiToken apiToken = new ApiToken();

            try
            {
                //get the comp on token
                apiToken = await _DBContext.ApiToken.Where(x => x.Token == token).FirstOrDefaultAsync();

                if (apiToken != null)
                {
                    //get reg info
                    var regInfo = await _DBContext.TblVersion.Where(x => x.CompanyId == apiToken.CompanyId &&
                                                            x.DivisionId == apiToken.DivisionId &&
                                                            x.DepartmentId == apiToken.DepartmentId).FirstOrDefaultAsync();
                    if (regInfo != null)
                    {
                        days = EnterpriseValidator.GetDaysLeft(regInfo.RegCode, regInfo.RegName);
                        apiToken.RegCode = regInfo.RegCode;
                        apiToken.RegCode = regInfo.RegName;
                        apiToken.TotalDays = days;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return apiToken;
        }

        public async Task<IEnumerable<PayrollHrpayrollAttDetail>> GetAttendance(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollAttDetail.OrderByDescending(x => x.AttendanceDate).Where(x => 
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PayrollHrpayrollAttDetail>> GetByEmployee(string Id, string Mode, ApiToken token)
        {
            try
            {
                if (Mode == "Current")
                {
                    return await _DBContext.PayrollHrpayrollAttDetail.OrderByDescending(x => x.AttendanceDate).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == Id &&
                                                            x.AttendanceDate == DateTime.Now.Date).ToListAsync();
                }
                else if (Mode == "History")
                {
                    return await _DBContext.PayrollHrpayrollAttDetail.OrderByDescending(x => x.AttendanceDate).Where(x => x.CompanyId == token.CompanyId &&
                                                             x.DivisionId == token.DivisionId &&
                                                             x.DepartmentId == token.DepartmentId &&
                                                             x.EmployeeId == Id &&
                                                             x.AttendanceDate < DateTime.Now.Date).ToListAsync();
                }
                else
                {
                    return await _DBContext.PayrollHrpayrollAttDetail.OrderByDescending(x => x.AttendanceDate).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == Id).ToListAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PayrollHrpayrollAttDetail> GetById(string Id, DateTime AttendanceDate, ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollAttDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeId == Id &&
                                                        x.AttendanceDate == AttendanceDate.Date).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DeductionSummary>> GetDeductionReport(DateTime PeriodFrom, DateTime PeriodTo, ApiToken token)
        {
            List<DeductionSummary> deductionReportSummary = new List<DeductionSummary>();
            try
            {
                var companyStandard = await _DBContext.PayrollHrpayrollCompanyStandard.Where(x => x.CompanyId == token.CompanyId &&
                                        x.DivisionId == token.DivisionId &&
                                        x.DepartmentId == token.DepartmentId).FirstOrDefaultAsync();

                var attendance = await _DBContext.PayrollHrpayrollAttDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                     x.DivisionId == token.DivisionId &&
                                                     x.DepartmentId == token.DepartmentId &&
                                                     x.AttendanceDate.Date >= PeriodFrom.Date &&
                                                     x.AttendanceDate.Date <= PeriodTo.Date).ToListAsync();

                var attendanceDates = (from att in attendance select att.AttendanceDate.Date).Distinct();
                var employeeList = await _employee.GetAll(null, token);
                //List<DeductionSummary> deductionReportSummary = new List<DeductionSummary>();

                foreach (var employee in employeeList)
                {
                    int totalAbsentDays = 0;
                    int totalMinutes = 0;
                    foreach (DateTime dates in attendanceDates)
                    {
                        var attDates = attendance.Where(x => x.EmployeeId == employee.EmployeeId &&
                                                        x.AttendanceDate.Date == dates.Date).FirstOrDefault();
                        if (attDates == null)
                        {
                            totalAbsentDays += 1;
                            totalMinutes += 540;//Coverts to mins
                        }
                        if (attDates != null)
                        {
                            totalAbsentDays += 0;
                            totalMinutes += 0;

                            if (attDates.TimeIn.Value.TimeOfDay > companyStandard.GraceTime.Value.TimeOfDay)
                            {
                                //get difference in datetime between grace period and time in
                                var timeDiff = attDates.TimeIn.Value.TimeOfDay - companyStandard.GraceTime.Value.TimeOfDay;
                                int lateHr = Convert.ToInt32(timeDiff.Hours == null || timeDiff.Hours == 0 ? 0 :
                                  timeDiff.Hours * 60);
                                int lateMin = Convert.ToInt32(timeDiff.Minutes == null || timeDiff.Minutes == 0 ? 0 :
                                     timeDiff.Minutes);
                                int lateSec = Convert.ToInt32(timeDiff.Seconds == null || timeDiff.Seconds == 0 ? 0 :
                                    timeDiff.Seconds * 0.0166666666666667);
                                totalMinutes += lateHr + lateMin + lateSec;

                            }
                        }
                    }
                    if (totalAbsentDays > 0 || totalMinutes > 0)
                    {
                        DeductionSummary deductionRepSum = new DeductionSummary();
                        deductionRepSum.Name = employee.EmployeeName;
                        deductionRepSum.EmployeeId = employee.EmployeeId;
                        deductionRepSum.TotalAbsentDays = totalAbsentDays;
                        deductionRepSum.TotalMinutes = totalMinutes;
                        deductionReportSummary.Add(deductionRepSum);
                    }
                }
            }
            catch (Exception ex)
            {

            }            
            return deductionReportSummary;
        }
    


        public async Task<IEnumerable<LatenessReportDetails>> GetLatenessReportDetails(DateTime periodFrom, DateTime periodTo, ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollAttDetail.Where(x => x.CompanyId == token.CompanyId &&
                                        x.DivisionId == token.DivisionId &&
                                        x.DepartmentId == token.DepartmentId)
                                        .Where(x => x.TimeIn.Value > x.ExpectedTimeIn)
                                        .Select(x => new LatenessReportDetails
                                        {
                                            Date = x.SystemDate.Value,
                                            EmployeeId = x.EmployeeId,
                                            Name = x.EmployeeName,
                                            FirstName = x.EmployeeFirstName,
                                                        //LastName = x.EmployeeLastName,
                                            TimeInMinutes = (DateTime.Parse(x.TimeIn.Value.ToString()) -
                                            DateTime.Parse(x.ExpectedTimeIn.Value.ToString())).TotalMinutes
                                        }).ToListAsync();
            }
            catch (Exception ex)
            {

            }
            return null;

        }

        //public async Task<IEnumerable<LatenessReportSummary>> GetLatenessReportSummary(DateTime PeriodFrom, DateTime PeriodTo, ApiToken token)
        //{
        //    return await _DBContext.PayrollHrpayrollAttDetail.Where(x => x.CompanyId == token.CompanyId &&
        //                                             x.DivisionId == token.DivisionId &&
        //                                             x.DepartmentId == token.DepartmentId)
        //                                             .Where(x => x.TimeIn.Value > x.ExpectedTimeIn)
        //                                             .Select(x => new LatenessReportSummary
        //                                             {
        //                                                 EmployeeId = x.EmployeeId,
        //                                                 Name = x.EmployeeName,
        //                                                 Minutes = (DateTime.Parse(x.TimeIn.Value.ToString()) -
        //                                                 DateTime.Parse(x.ExpectedTimeIn.Value.ToString())).TotalMinutes
        //                                             }).ToListAsync();
        //}

        public async Task<IEnumerable<LatenessReportSummary>> GetLatenessReportSummary(DateTime PeriodFrom, DateTime PeriodTo, ApiToken token)
        {
            //List<LatenessReportSummary> LatenessReportSum = new List<LatenessReportSummary>();
            try
            {
                var companyStandard = await _DBContext.PayrollHrpayrollCompanyStandard.Where(x => x.CompanyId == token.CompanyId &&
                                        x.DivisionId == token.DivisionId &&
                                        x.DepartmentId == token.DepartmentId).FirstOrDefaultAsync();

                var lateness = await _DBContext.PayrollHrpayrollAttDetail.Where(x => x.CompanyId == token.CompanyId &&
                                        x.DivisionId == token.DivisionId &&
                                        x.DepartmentId == token.DepartmentId &&
                                        x.TimeIn.Value.Hour >= companyStandard.GraceTime.Value.Hour &&
                                        x.TimeIn.Value.Minute >= companyStandard.GraceTime.Value.Minute ||
                                        x.TimeIn.Value.Hour > companyStandard.GraceTime.Value.Hour).ToListAsync();
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<IEnumerable<AbsenteeismReportDetail>> GetAbsenteeismReportDetail(ApiToken token)
        {
            return await _DBContext.PayrollHrpayrollAttDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                               x.DivisionId == token.DivisionId &&
                                                               x.DepartmentId == token.DepartmentId)
                                                               .Where(x => x.ClockedIn == null || x.ClockedIn == false)
                                                               .Where(x => x.SystemDate.Value.ToString() != null)
                                                               .Select(x => new AbsenteeismReportDetail
                                                               {
                                                                   EmployeeId = x.EmployeeId,
                                                                   Name = x.EmployeeName,
                                                                   FirstName = x.EmployeeFirstName,
                                                                   LastName = x.EmployeeLastName,
                                                                   DateAbsent = (DateTime.Parse(x.SystemDate.Value.ToString())).Date
                                                               }).ToListAsync();
        }

        public async Task<IEnumerable<AbsenteeismReportSummary>> GetAbsenteeismReportSummary(DateTime PeriodFrom, DateTime PeriodTo, ApiToken token)
        {
            List<AbsenteeismReportSummary> AbsReportSum = new List<AbsenteeismReportSummary>();
            try
            {
                var attendance = await _DBContext.PayrollHrpayrollAttDetail.Where(x => x.CompanyId == token.CompanyId &&
                                        x.DivisionId == token.DivisionId &&
                                        x.DepartmentId == token.DepartmentId &&
                                        x.AttendanceDate >= PeriodFrom.Date &&
                                        x.AttendanceDate <= PeriodTo.Date).ToListAsync();

                var attendanceDates = (from att in attendance
                                       select att.AttendanceDate.Date).Distinct();


                var empList = await _employee.GetAll(null, token);

                
                foreach (PayEmployees employee in empList)
                {
                    int totalAbsentDays = 0;

                    foreach (DateTime dates in attendanceDates)
                    {
                        var attendedDates = attendance
                                               .Where(x => x.EmployeeId == employee.EmployeeId &&
                                               x.AttendanceDate.Date == dates.Date).FirstOrDefault();

                        if (attendedDates == null)
                        {
                            totalAbsentDays += 1;
                        }
                    }

                    if (totalAbsentDays > 0)
                    {
                        AbsenteeismReportSummary absenteeRepSum = new AbsenteeismReportSummary();
                        absenteeRepSum.Name = employee.EmployeeName;
                        absenteeRepSum.EmployeeId = employee.EmployeeId;
                        absenteeRepSum.TotalAbsentDays = totalAbsentDays;
                        AbsReportSum.Add(absenteeRepSum);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return AbsReportSum;
        }

        public async Task<IEnumerable<DailyAttendance>> GetDailyAttendanceSummary(DateTime PeriodFrom, DateTime PeriodTo, ApiToken token)
        {
            List<DailyAttendance> dailyAttendance = new List<DailyAttendance>();
            try
            {
                dailyAttendance = await _DBContext.EtlPayrollHrpayrollAttSummaryDaily.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.AttendanceDate.Date >= PeriodFrom &&
                                                        x.AttendanceDate.Date <= PeriodTo)
                                                        .Select(x => new DailyAttendance
                                                        {
                                                            CompanyId = x.CompanyId,
                                                            DivisionId = x.DivisionId,
                                                            DepartmentId = x.DepartmentId,
                                                            EmployeeFirstName = x.EmployeeFirstName,
                                                            EmployeeName = x.EmployeeName,
                                                            EmployeeId = x.EmployeeId,
                                                            AttendanceDate = x.AttendanceDate.Date,
                                                            ExpectedNoofWorkHrs = x.ExpectedNoofWorkHrs,
                                                            ExpectedTimeIn = x.ExpectedTimeIn,
                                                            ExpectedTimeOut = x.ExpectedTimeOut,
                                                            WorkedHours = x.Workedhours,
                                                            Paidhours = x.PaidHours,
                                                            AbsentHours = x.AbsentHours,
                                                            TimeIn = x.TimeIn,
                                                            ClockedOut = x.ClockedOut
                                                        }).ToListAsync();
            }
            catch (Exception ex)
            {
                
            }
            return dailyAttendance;
        }

        public async Task<IEnumerable<MonthlyAttendance>> GetMonthlyAttendanceSummary(DateTime PeriodFrom, DateTime PeriodTo, ApiToken token)
        {
            List<MonthlyAttendance> monthlyAttendance = new List<MonthlyAttendance>();
            try
            {
                monthlyAttendance = await _DBContext.EtlPayrollHrpayrollAttSummaryMonthly.Where(x => x.CompanyId == token.CompanyId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.AttendanceDate.Date >= PeriodFrom &&
                                                       x.AttendanceDate.Date <= PeriodTo)
                                                       .Select(x => new MonthlyAttendance
                                                       {
                                                           CompanyId = x.CompanyId,
                                                           DivisionId = x.DivisionId,
                                                           DepartmentId = x.DepartmentId,
                                                           EmployeeFirstName = x.EmployeeFirstName,
                                                           EmployeeName = x.EmployeeName,
                                                           EmployeeId = x.EmployeeId,
                                                           AttendanceDate = x.AttendanceDate,
                                                           ExpectedNoofWorkHrs = x.ExpectedNoofWorkHrs,
                                                           WorkedHours = x.Workedhours,
                                                           Paidhours = x.PaidHours,
                                                           AbsentHours = x.AbsentHours,
                                                           ExpectedNoofDays = x.ExpectedNoofDays,
                                                           WorkedDays = x.WorkedDays
                                                       }).ToListAsync();
            }
            catch(Exception ex)
            {

            }
            return monthlyAttendance;
        }
           


        //public Task<StatusMessage> Process(string Id, string type, ApiToken token)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<StatusMessage> Update(PayrollHrpayrollAttDetail attendance, ApiToken token)
        //{
        //    throw new NotImplementedException();
        //}
    }
}



   

