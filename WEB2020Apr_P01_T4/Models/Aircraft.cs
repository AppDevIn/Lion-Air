﻿using System;
using System.ComponentModel.DataAnnotations;

namespace WEB2020Apr_P01_T4.Models
{
    public class Aircraft
    {
        
        public int AircraftID { get; set; }

        
        public String AircraftModel { get; set; }
        
        public int NumEconomySeat { get; set; }

        public int NumBusinessSeat { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateLastMaintenance { get; set; }

        public String Status { get; set; }


        public Aircraft()
        {
        }
    }
}