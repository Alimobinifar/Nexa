using Domain.Entities.BaseEntities;
using Domain.Entities.Users;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.SystemLogs
{
    public class Logs : BaseModel
    {
        /// <summary>
        /// نوع عملیات: Create, Update, Delete, Login, Logout و ...
        /// </summary>
        public LogActionType ActionType{ get; set; }

        /// <summary>
        /// توضیحات عملیات
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// تاریخ شمسی
        /// </summary>
        public string PersianDate { get; set; }


        /// <summary>
        /// شناسه کاربری که عملیات را انجام داده
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// نام بخش یا ماژول سیستم
        /// </summary>
        public string? Module { get; set; }

        /// <summary>
        /// سطح اهمیت لاگ: Info, Warning, Error, Critical
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// جزئیات خطا در صورت وجود
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// آدرس IP کاربر
        /// </summary>
        public string? IpAddress { get; set; }

        #region Navigation Properties
        public User User { get; set; }
        #endregion
    }


}
