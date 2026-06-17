import { Component } from "@angular/core";
import { Router } from "@angular/router";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"],
})
export class HomeComponent {
  categories = [
    { name: "Headphones", icon: "🎧", description: "Earbuds, Over-ear, ANC" },
    {
      name: "Smartphones",
      icon: "📱",
      description: "iPhone, Samsung, OnePlus",
    },
    {
      name: "Laptops",
      icon: "💻",
      description: "Gaming, Business, Ultrabooks",
    },
    { name: "Tablets", icon: "📟", description: "iPad, Galaxy Tab, OnePlus" },
    { name: "Smartwatches", icon: "⌚", description: "Apple, Samsung, Garmin" },
    { name: "Cameras", icon: "📷", description: "DSLR, Mirrorless, Action" },
    { name: "Gaming", icon: "🎮", description: "PS5, Xbox, Accessories" },
    { name: "Monitors", icon: "🖥️", description: "4K, Gaming, OLED" },
  ];

  constructor(private router: Router) {}

  goToProducts(): void {
    this.router.navigate(["/products"]);
  }
}
