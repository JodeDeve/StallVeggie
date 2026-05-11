// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Sidebar toggle for small screens
document.addEventListener("DOMContentLoaded", function () {
  var toggle = document.querySelector(".sidebar-toggle");
  var overlay = document.getElementById("sidebarOverlay");

  function openSidebar() {
    document.body.classList.add("sidebar-open");
    if (overlay) overlay.classList.add("show");
  }

  function closeSidebar() {
    document.body.classList.remove("sidebar-open");
    if (overlay) overlay.classList.remove("show");
  }

  if (toggle) {
    toggle.addEventListener("click", function (e) {
      e.preventDefault();
      if (document.body.classList.contains("sidebar-open")) closeSidebar();
      else openSidebar();
    });
  }

  if (overlay) {
    overlay.addEventListener("click", function () {
      closeSidebar();
    });
  }

  // Close sidebar when a nav link is clicked (mobile)
  document.querySelectorAll(".sidebar-nav .nav-link").forEach(function (el) {
    el.addEventListener("click", function () {
      closeSidebar();
    });
  });
});

// Additional site JS can be added below
