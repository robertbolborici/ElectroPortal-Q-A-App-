import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['emailConfirmation']) {
        if (params['emailConfirmation'] === 'Success') {
          alert('Your email has been successfully confirmed. You may now login.');
        } else if (params['emailConfirmation'] === 'Failure') {
          alert('There was an error confirming your email. Please try the link again or contact support.');
        } else if (params['emailConfirmation'] === 'NotFound') {
          alert('The user for this confirmation does not exist.');
        }
      }
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      const { username, password } = this.loginForm.value;
      this.authService.login(username, password).subscribe({
        next: (response) => {
          localStorage.setItem('jwtToken', response.token);
          this.authService.setLogin();
          this.router.navigate(['/home']);
        },
        error: (error) => {
          alert(error.error.message || 'Login failed');
        }
      });
    }
  }
}
