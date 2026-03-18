using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VS_portfolio_2026.Models;

namespace VS_portfolio_2026.Controllers
{
    public class PortfolioController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewData["Title"] = "My Portfolio - Featured Projects";
            ViewData["Description"] = "Explore a collection of high-performance projects by Abu Huraira, including SaaS applications, E-commerce platforms, and real-time chat systems.";
            ViewData["Keywords"] = "portfolio, software projects, web development, SaaS, MERN stack, .NET projects, Abu Huraira";
            return View();
        }

        public IActionResult Nexus()
        {
            ViewData["Title"] = "Nexus - Collaborative Whiteboard SaaS";
            ViewData["Description"] = "A detailed look at Nexus, a solo-founded digital whiteboard application built with MERN stack and TypeScript for visual brainstorming.";
            ViewData["Keywords"] = "Nexus, digital whiteboard, SaaS, MERN stack, TypeScript, canvas tools, Abu Huraira";
            return View();
        }

        public IActionResult Chromaic()
        {
            ViewData["Title"] = "Chromaic - Modern E-commerce Store";
            ViewData["Description"] = "Explore Chromaic, a modern MERN stack e-commerce ecosystem designed for high-performance retail and seamless user experiences.";
            ViewData["Keywords"] = "Chromaic, e-commerce, MERN stack, React store, online shop, Abu Huraira";
            return View();
        }

        public IActionResult Talksphere()
        {
            ViewData["Title"] = "TalkSphere - Real-Time Chat Platform";
            ViewData["Description"] = "Dive into TalkSphere, a real-time communication platform using WebSockets for instant connectivity and seamless messaging.";
            ViewData["Keywords"] = "Talksphere, chat application, WebSocket, real-time communication, Java, full-stack chat";
            return View();
        }

        public IActionResult Cartnova()
        {
            ViewData["Title"] = "CartNova - High-Conversion E-commerce";
            ViewData["Description"] = "Discover CartNova, a specialized e-commerce architecture optimized for high conversion rates and responsive performance.";
            ViewData["Keywords"] = "CartNova, e-commerce architecture, JavaScript projects, responsive design, online retail";
            return View();
        }

        public IActionResult ResQ()
        {
            ViewData["Title"] = "ResQ - Healthcare Management System";
            ViewData["Description"] = "An overview of ResQ, a full-stack management system designed for elderly care and healthcare facility organization.";
            ViewData["Keywords"] = "ResQ, healthcare system, elderly care, management software, full-stack development";
            return View();
        }

        public IActionResult Croissocafe()
        {
            ViewData["Title"] = "CroissoCafe - Brand Identity Design";
            ViewData["Description"] = "A showcase of CroissoCafe, a conceptual brand identity and highly responsive landing page project.";
            ViewData["Keywords"] = "CroissoCafe, branding, landing page, web design, responsive UI, brand identity";
            return View();
        }

        public IActionResult Sahayta()
        {
            ViewData["Title"] = "Sahayta - Social Support Platform";
            ViewData["Description"] = "Learn about Sahayta, a foundation platform built to facilitate social support, donations, and community outreach.";
            ViewData["Keywords"] = "Sahayta, social platform, donation website, community support, Java web development";
            return View();
        }

        public IActionResult RunQuest()
        {
            ViewData["Title"] = "RunQuest - AI-Driven Fitness Game";
            ViewData["Description"] = "Explore RunQuest, an innovative fitness ecosystem using AI to gamify exercise and promote active lifestyles.";
            ViewData["Keywords"] = "RunQuest, fitness game, AI fitness, gamified exercise, Python projects, health tech";
            return View();
        }
    }
}
