import { Component } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

@Component({
  selector: "app-contact",
  templateUrl: "./contact.component.html",
  styleUrls: ["./contact.component.css"],
})
export class ContactComponent {
  form: FormGroup;
  sent = false;
  loading = false;
  faqs = [
    {
      q: "How long does delivery take?",
      a: "Standard delivery takes 3–5 business days.",
    },
    {
      q: "What is your return policy?",
      a: "30-day hassle-free returns on all products.",
    },
    {
      q: "Is my payment secure?",
      a: "Yes, we use industry-standard SSL encryption.",
    },
    {
      q: "Do you ship internationally?",
      a: "Currently we ship within India only.",
    },
  ];
  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: ["", Validators.required],
      email: ["", [Validators.required, Validators.email]],
      subject: ["", Validators.required],
      message: ["", [Validators.required, Validators.minLength(20)]],
    });
  }
  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    setTimeout(() => {
      this.sent = true;
      this.loading = false;
    }, 1000);
  }
}
