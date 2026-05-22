using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Utilities
{
    public static class EmailTemplates
    {
        /*
        =========================================
        WELCOME EMAIL
        =========================================
        */

        public static string WelcomeEmployee(
            string firstName,
            string companyName,
            string Email,
            string Password
        )
        {
            var body = $@"
                <p>Hello {firstName},</p>

                <p>
                    Welcome to <strong>{companyName}</strong>.
                </p>

                <p>
                    Your employee account has been created successfully.
                </p>
                 <p>
                    Kindly use the the below credentials to login to your employee portal at www.zucohr.com and don't forget to change your password.
                </p>
                 <p>
                    {Email}
                    {Password}
                </p>
                <p>
                    We are excited to have you onboard.
                </p>
            ";

            return EmailLayout.Build(
                "Welcome to ZucoHR",
                body
            );
        }

        /*
        =========================================
        LEAVE APPROVED
        =========================================
        */

        public static string LeaveApproved(
            string firstName,
            string leaveType,
            int days
        )
        {
            var body = $@"
                <p>Hello {firstName},</p>

                <p>
                    Your leave request has been approved.
                </p>

                <table cellpadding='6'>
                    <tr>
                        <td><strong>Type:</strong></td>
                        <td>{leaveType}</td>
                    </tr>

                    <tr>
                        <td><strong>Duration:</strong></td>
                        <td>{days} days</td>
                    </tr>
                </table>
            ";

            return EmailLayout.Build(
                "Leave Approved",
                body
            );
        }

        /*
        =========================================
        LEAVE REJECTED
        =========================================
        */

        public static string LeaveRejected(
            string firstName,
            string leaveType
        )
        {
            var body = $@"
                <p>Hello {firstName},</p>

                <p>
                    Unfortunately, your {leaveType}
                    leave request was rejected.
                </p>

                <p>
                    Please contact HR for clarification.
                </p>
            ";

            return EmailLayout.Build(
                "Leave Request Rejected",
                body
            );
        }

        /*
        =========================================
        PAYROLL PROCESSED
        =========================================
        */

        public static string PayrollProcessed(
            string firstName,
            decimal amount,
            string month
        )
        {
            var body = $@"
                <p>Hello {firstName},</p>

                <p>
                    Your payroll for <strong>{month}</strong>
                    has been processed.
                </p>

                <div style='
                    background:#ecfdf5;
                    padding:20px;
                    border-radius:10px;
                    margin-top:20px;
                '>

                    <h2 style='margin:0;color:#059669;'>
                        ₦{amount:N2}
                    </h2>

                </div>
            ";

            return EmailLayout.Build(
                "Payroll Processed",
                body
            );
        }

        /*
        =========================================
        TASK ASSIGNED
        =========================================
        */

        public static string TaskAssigned(
            string firstName,
            string taskTitle,
            DateTime dueDate
        )
        {
            var body = $@"
                <p>Hello {firstName},</p>

                <p>
                    A new task has been assigned to you.
                </p>

                <div style='
                    border-left:4px solid #2563eb;
                    padding-left:16px;
                    margin:20px 0;
                '>

                    <p>
                        <strong>Task:</strong>
                        {taskTitle}
                    </p>

                    <p>
                        <strong>Due Date:</strong>
                        {dueDate:MMMM dd, yyyy}
                    </p>

                </div>
            ";

            return EmailLayout.Build(
                "New Task Assigned",
                body
            );
        }

        /*
        =========================================
        PASSWORD RESET
        =========================================
        */

        public static string PasswordReset(
            string firstName,
            string resetLink
        )
        {
            var body = $@"
                <p>Hello {firstName},</p>

                <p>
                    We received a request to reset your password.
                </p>

                <p style='margin-top:30px;'>

                    <a href='{resetLink}'
                       style='
                        background:#2563eb;
                        color:#fff;
                        padding:12px 20px;
                        text-decoration:none;
                        border-radius:8px;
                        display:inline-block;
                       '>

                        Reset Password

                    </a>

                </p>

                <p style='margin-top:25px;font-size:13px;color:#6b7280;'>
                    If you did not request this,
                    please ignore this email.
                </p>
            ";

            return EmailLayout.Build(
                "Password Reset",
                body
            );
        }

        /*
        =========================================
        EXPENSE APPROVED
        =========================================
        */

        public static string ExpenseApproved(
            string firstName,
            decimal amount
        )
        {
            var body = $@"
                <p>Hello {firstName},</p>

                <p>
                    Your expense request has been approved.
                </p>

                <p>
                    Approved Amount:
                    <strong>₦{amount:N2}</strong>
                </p>
            ";

            return EmailLayout.Build(
                "Expense Approved",
                body
            );
        }

        /*
        =========================================
        ONBOARDING TASK
        =========================================
        */

        public static string OnboardingTask(
            string firstName,
            string taskTitle
        )
        {
            var body = $@"
                <p>Hello {firstName},</p>

                <p>
                    A new onboarding task has been assigned.
                </p>

                <p>
                    <strong>{taskTitle}</strong>
                </p>
            ";

            return EmailLayout.Build(
                "New Onboarding Task",
                body
            );
        }
    }
}
