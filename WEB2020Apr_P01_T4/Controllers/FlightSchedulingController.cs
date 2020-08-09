using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WEB2020Apr_P01_T4.DAL;
using WEB2020Apr_P01_T4.ViewModel;
using WEB2020Apr_P01_T4.Models;
using Microsoft.AspNetCore.Http;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WEB2020Apr_P01_T4.Controllers
{
    public class FlightSchedulingController : Controller
    {


        RouteDAL routeDAL = new RouteDAL();
        FlightScheduleDAL flightScheduleDAL = new FlightScheduleDAL();
        BookingDAL bookingDAL = new BookingDAL();

        private  IActionResult CheckAdmin(IActionResult view)
        {
            
            
            if (false)//((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Admin"))
            {

                return RedirectToAction("Index", "Home");

            }



            return view;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {

            RouteViewModel routeViewModel = new RouteViewModel
            {
                RouteList = routeDAL.getAllRoutes(),
                SearchOption = Route.GetTableList(),
                TicketSize = bookingDAL.GetAllBooking().Count(),
                FlightSchedule = new FlightSchedule(),
                CreateRoute = new Route(),
                ShowAddPop = false,
                ShowRoutePop = false
            };

            return CheckAdmin(View(routeViewModel));
            
        }

        [Route("FlightScheduling/Schedule")]
        public IActionResult Schedule()
        {

            ScheduleViewModel scheduleViewModel = new ScheduleViewModel
            {
                FlightSchedule = new FlightSchedule(),
                CreateRoute = new Route(),
                SearchOption = FlightSchedule.GetTableList(),
                FlightScheduleList = flightScheduleDAL.GetAllFlightSchedule(),
                ShowEditPop = false,
                TicketSize = bookingDAL.GetAllBooking().Count(), 

            };

            List<String> ecoOccupied = new List<string>();
            List<String> bOccupied = new List<string>();
            foreach (FlightSchedule flightSchedule in scheduleViewModel.FlightScheduleList)
            {
                if (flightSchedule.AircraftID != 0)
                {
                    List<int> values = flightScheduleDAL.GetSeats(flightSchedule.AircraftID);
                    ecoOccupied.Add(flightScheduleDAL.CountEconomySeat(flightSchedule.ScheduleID).ToString() + "/" + values[0]);
                    bOccupied.Add(flightScheduleDAL.CountBusinessSeat(flightSchedule.ScheduleID).ToString() + "/" + values[1]);
                }
                else
                {
                    ecoOccupied.Add("Currently No Aircarft ID");
                    bOccupied.Add("Currently No Aircarft ID");
                }

            }

            ViewBag.ecoOccupied = ecoOccupied;
            ViewBag.bOccupied = bOccupied;


            ViewBag.Flitered = false;



            return CheckAdmin(View("Schedule", scheduleViewModel));
        }

        [Route("FlightScheduling/Schedule/{id}")]
        public IActionResult Schedule(int id)
        {

            ScheduleViewModel scheduleViewModel = new ScheduleViewModel
            {
                FlightSchedule = new FlightSchedule(),
                CreateRoute = new Route(),
                SearchOption = FlightSchedule.GetTableList(),
                FlightScheduleList = flightScheduleDAL.GetAllFlightSchedule(id),
                ShowEditPop = false,
                TicketSize = bookingDAL.GetAllBooking().Count(),

            };

            List<String> ecoOccupied = new List<string>();
            List<String> bOccupied = new List<string>();

            if (scheduleViewModel.FlightScheduleList != null)
            {
                foreach (FlightSchedule flightSchedule in scheduleViewModel.FlightScheduleList)
                {
                    if (flightSchedule.AircraftID != 0)
                    {
                        List<int> values = flightScheduleDAL.GetSeats(flightSchedule.AircraftID);
                        ecoOccupied.Add(flightScheduleDAL.CountEconomySeat(flightSchedule.ScheduleID).ToString() + "/" + values[0]);
                        bOccupied.Add(flightScheduleDAL.CountBusinessSeat(flightSchedule.ScheduleID).ToString() + "/" + values[1]);
                    }
                    else
                    {
                        ecoOccupied.Add("Currently No Aircarft ID");
                        bOccupied.Add("Currently No Aircarft ID");
                    }

                }

                ViewBag.ecoOccupied = ecoOccupied;
                ViewBag.bOccupied = bOccupied;
            }

            ViewBag.Flitered = true;


            return CheckAdmin(View("Schedule", scheduleViewModel));
        }



        public IActionResult FlightBooking(int id)
        {



            
            BookingViewModel bookingViewModel = new BookingViewModel
            {
                BookingList = bookingDAL.GetAllBooking().Where(booking => booking.ScheduleID == id),
                CreateRoute = new Route(),
                SearchOption = Booking.GetTableList(),
                FlightSchedule = flightScheduleDAL.GetFlightSchedule(id)

            };

            return CheckAdmin(View(bookingViewModel));
        }


        
 
        
        public ActionResult EditSchedule(int id)
        {
            //stuff you need and then return the partial view 
            //I recommend using "" quotes for a partial view
            FlightSchedule fs = flightScheduleDAL.GetFlightSchedule(id);
            return PartialView("_ScheduleForm", fs);
        }



        public ActionResult AddSchedule(int id)
        {
            //stuff you need and then return the partial view 
            //I recommend using "" quotes for a partial view
            FlightSchedule fs = new FlightSchedule();
            fs.RouteID = id;
            fs.Status = "Opened";
            return PartialView("_ScheduleForm", fs);
        }

        public ActionResult EditRoute(int id)
        {
            //stuff you need and then return the partial view 
            //I recommend using "" quotes for a partial view
            Route route = routeDAL.getAllRoutes().First(r => r.RouteID == id);
            return PartialView("_RouteForm", route);
        }

        [HttpPost]
        public IActionResult SaveRoute(Route route)
        {
            bool isValid = true;
            if (ModelState.IsValid)
            {

                //Insert the data
                ViewData["routeExist"] = routeDAL.InsertData(route);
                isValid = (bool)ViewData["routeExist"];
                

            }
            else
            {
                isValid = false;
                ViewData["routeExist"] = true;
            }


            if (isValid)
            {

                return RedirectToAction("Index");

            }
            else
            {
                RouteViewModel routeViewModel = new RouteViewModel
                {
                    RouteList = routeDAL.getAllRoutes(),
                    SearchOption = Route.GetTableList(),
                    TicketSize = bookingDAL.GetAllBooking().Count(),
                    FlightSchedule = new FlightSchedule(),
                    CreateRoute = new Route(),
                    ShowAddPop = false,
                    ShowRoutePop = true
                };



                return View("Index", routeViewModel);
            }


        }



        [HttpPost]
        [Route("FlightScheduling/SaveSchedule/{id}")]
        public IActionResult SaveSchedule(FlightSchedule flightSchedule, int id)
        {
            int RouteID = id;

            DateTime? ArrivalDateTime = null;


            if (ModelState.IsValid)
            {

                Route route = routeDAL.getAllRoutes().First(r => r.RouteID == id);

                if (flightSchedule.DepartureDateTime != null)
                {
                    ArrivalDateTime = ((DateTime)flightSchedule.DepartureDateTime).AddHours((double)route.FlightDuration);
                }
               

                flightSchedule.RouteID = id;
                flightSchedule.ArrivalDateTime = ArrivalDateTime;


                //Insert the data
                flightScheduleDAL.InsertData(flightSchedule);
                return RedirectToAction("Index");
            }
            else
            {
           

                RouteViewModel routeViewModel = new RouteViewModel
                {
                    RouteList = routeDAL.getAllRoutes(),
                    SearchOption = Route.GetTableList(),
                    TicketSize = bookingDAL.GetAllBooking().Count(),
                    FlightSchedule = new FlightSchedule(),
                    CreateRoute = new Route(),
                    ShowAddPop = true
                };

                //Set the route ID again
                routeViewModel.FlightSchedule.RouteID = RouteID;

                return View("Index", routeViewModel);
                

    
            }


        }

        [HttpPost]
        public IActionResult UpdateSchedule(FlightSchedule flightSchedule)
        {

            //Insert the data
            flightScheduleDAL.Update(flightSchedule);

            return RedirectToAction("Schedule");
        }


       
        [Route("FlightScheduling/DeleteSchedule/{id}/{routeID}")]
        public IActionResult DeleteSchedule(int id, int routeID)
        {

            //Delete the schdule
            flightScheduleDAL.Delete(id);

            if (routeID == 0)
            {
                return RedirectToAction("Schedule");
            }
            else
            {
                return RedirectToAction("Schedule", new { id = routeID});
            }

        }
    }
}
