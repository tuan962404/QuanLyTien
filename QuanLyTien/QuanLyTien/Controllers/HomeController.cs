using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyTien.Controllers
{
    public class HomeController : BaseController
    {
        QuanLyTienEntities db = new QuanLyTienEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List_Name_Hui()
        {
            ViewBag.List = db.List_Name_HuiHeo.OrderByDescending(x=>x.Id).ToList();
            return View();
        }

        public ActionResult List_Name_TienGop()
        {
            ViewBag.List = db.List_Name_TienGop.OrderByDescending(x => x.Id).ToList();
            return View();
        }

        public ActionResult AddNameHui()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNameHui(List_Name_HuiHeo hui)
        {
            if(hui != null)
            {
                var kq = db.List_Name_HuiHeo.Find(hui.Name);
                if(kq != null)
                {
                    ViewBag.Error = "";
                    return View();
                }
                else
                {
                    List_Name_HuiHeo list = new List_Name_HuiHeo();
                    list.Name = hui.Name;
                    list.Buoi = hui.Buoi;
                    list.TieuDe = hui.TieuDe;
                    db.List_Name_HuiHeo.Add(list);
                    db.SaveChanges();
                    return RedirectToAction("List_Name_Hui");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult DeleteNameHui(string name)
        {
            if(name != "" || name != null)
            {
                var kq = db.List_Name_HuiHeo.FirstOrDefault(x=>x.Name == name);
                var list = db.Detail_HuiHeo.Where(x => x.Name == name).ToList();
                foreach(var i in list)
                {
                    Detail_Remove_HuiHeo mod = new Detail_Remove_HuiHeo();
                    mod.Name = i.Name;
                    mod.Date = i.Date;
                    mod.Buoi = i.Buoi;
                    mod.SoTien = i.SoTien;
                    mod.GhiChu = i.GhiChu;
                    mod.TieuDe = kq.TieuDe;
                    mod.Status = true;
                    db.Detail_Remove_HuiHeo.Add(mod);

                    var Name = db.Detail_HuiHeo.Where(x=>x.Name == i.Name).FirstOrDefault();
                    db.Detail_HuiHeo.Remove(Name);
                    db.SaveChanges();
                }
                if(kq != null)
                {
                    List_Remove_HuiHeo da = new List_Remove_HuiHeo();
                    da.Name = kq.Name;
                    da.Buoi = kq.Buoi;
                    da.TieuDe = kq.TieuDe;
                    db.List_Remove_HuiHeo.Add(da);
                    db.List_Name_HuiHeo.Remove(kq);
                }
                db.SaveChanges();
                return RedirectToAction("List_Name_Hui");
            }
            else
            {
                return RedirectToAction("List_Name_Hui");
            }
        }

        public ActionResult DetailHui(string name,string tieude)
        {
            ViewBag.TieuDe = tieude;
            ViewBag.Name = name;
            ViewBag.List = db.Detail_HuiHeo.Where(x=>x.Name == name).ToList();
            return View();
        }

        public ActionResult View_Update_Hui(Detail_HuiHeo dt)
        {
            string date_format = String.Format("{0:yyyy-MM-dd}", dt.Date);
            ViewBag.Date = date_format;
            ViewBag.DateNonFortmat = dt.Date;
            ViewBag.List = db.List_Name_HuiHeo.ToList();
            return View();
        }

        [HttpGet]
        public JsonResult Update_Hui(string name, DateTime date, string buoi,int sotien,string ghichu)
        {
            var kq = db.Detail_HuiHeo.Where(x => x.Name == name && x.Date == date).FirstOrDefault();
            if(kq == null)
            {
                Detail_HuiHeo dt = new Detail_HuiHeo();
                dt.Name = name;
                dt.Date = date;
                dt.Buoi = buoi;
                dt.SoTien = sotien;
                dt.GhiChu = ghichu;
                dt.Status = false;
                db.Detail_HuiHeo.Add(dt);
                db.SaveChanges();
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
            else
            {
                kq.Name = name;
                kq.Date = date;
                kq.Buoi = buoi;
                kq.SoTien = sotien;
                kq.GhiChu = ghichu;
                kq.Status = false;
                db.SaveChanges();
                return Json(kq, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewNameRemoveHui()
        {
            ViewBag.List = db.List_Remove_HuiHeo.OrderByDescending(x => x.Id).ToList();
            return View();
        }

        public ActionResult DetailRemoveHui(string name, string tieude,string buoi)
        {
            ViewBag.Name = name;
            ViewBag.TieuDe = tieude;
            ViewBag.Buoi = buoi;
            ViewBag.List = db.Detail_Remove_HuiHeo.Where(x => x.Name == name && x.TieuDe == tieude && x.Buoi == buoi).ToList();
            return View();
        }

        public ActionResult AddNameTienGop()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNameTienGop(List_Name_TienGop mod,FormCollection collect)
        {
            int songay = Convert.ToInt32(collect["SoNgay"]);
            DateTime dateStart = Convert.ToDateTime(collect["NgayBatDau"]);
            DateTime dateEnd = dateStart.AddDays(songay - 1);
            int temp = dateStart.Day;
            int n = temp + songay;

            if (mod != null && collect != null)
            {
                var kq = db.List_Name_TienGop.Find(mod.Name);
                if (kq != null)
                {
                    ViewBag.Error = "";
                    return View();
                }
                else
                {
                    List_Name_TienGop list = new List_Name_TienGop();
                    list.Name = mod.Name;
                    list.PhoneNumber = mod.PhoneNumber;
                    list.TieuDe = mod.TieuDe;
                    db.List_Name_TienGop.Add(list);
                    
                    for (int i = temp;i < n; i++)
                    {
                        Detail_TienGop dt = new Detail_TienGop();
                        dt.Name = mod.Name;
                        dt.Date = dateStart;
                        dt.Status = false;
                        dateStart = dateStart.AddDays(1);
                        db.Detail_TienGop.Add(dt);
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                    return RedirectToAction("List_Name_TienGop");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult DetailTienGop(string name, string tieude)
        {
            ViewBag.TieuDe = tieude;
            ViewBag.Name = name;
            ViewBag.List = db.Detail_TienGop.Where(x => x.Name == name).ToList();
            return View();
        }

        public ActionResult View_Update_TienGop(string name,string tieude)
        {
            ViewBag.TieuDe = tieude;
            ViewBag.Name = name;
            ViewBag.List = db.Detail_TienGop.Where(x => x.Name == name).ToList();
            return View();
        }

        [HttpGet]
        public JsonResult UpdateTienGop(string name, DateTime date, int sotien, string ghichu)
        {
            var kq = db.Detail_TienGop.Where(x => x.Name == name && x.Date == date).FirstOrDefault();
            if (kq != null)
            {
                kq.Name = name;
                kq.Date = date;
                kq.SoTien = sotien;
                kq.GhiChu = ghichu;
                db.SaveChanges();
                return Json(kq, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTienGop(string name)
        {
            if (name != "" || name != null)
            {
                var kq = db.List_Name_TienGop.FirstOrDefault(x=>x.Name == name);
                var list = db.Detail_TienGop.Where(x => x.Name == name).ToList();
                foreach (var i in list)
                {
                    Detail_Remove_TienGop mod = new Detail_Remove_TienGop();
                    mod.Name = i.Name;
                    mod.Date = i.Date;
                    mod.SoTien = i.SoTien;
                    mod.GhiChu = i.GhiChu;
                    mod.TieuDe = kq.TieuDe;
                    mod.Status = true;
                    db.Detail_Remove_TienGop.Add(mod);

                    var Name = db.Detail_TienGop.Where(x => x.Name == i.Name).FirstOrDefault();
                    db.Detail_TienGop.Remove(Name);
                    db.SaveChanges();
                }

                if(kq != null)
                {
                    List_Remove_TienGop da = new List_Remove_TienGop();
                    da.Name = kq.Name;
                    da.TieuDe = kq.TieuDe;
                    db.List_Remove_TienGop.Add(da);
                    db.List_Name_TienGop.Remove(kq);
                }
                db.SaveChanges();
                return RedirectToAction("List_Name_TienGop");
            }
            else
            {
                return RedirectToAction("List_Name_TienGop");
            }
        }

        public ActionResult ViewNameTienGop()
        {
            ViewBag.List = db.List_Remove_TienGop.OrderByDescending(x => x.Id).ToList();
            return View();
        }

        public ActionResult DetailRemoveTienGop(string name, string tieude)
        {
            ViewBag.Name = name;
            ViewBag.TieuDe = tieude;
            ViewBag.List = db.Detail_Remove_TienGop.Where(x => x.Name == name && x.TieuDe == tieude).ToList();
            return View();
        }
    }
}