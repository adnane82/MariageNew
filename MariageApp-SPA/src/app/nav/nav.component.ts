
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
model:any={};
photoUrl:string;
  constructor( public authService:AuthService,private alertify:AlertifyService, private route:Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(
      photoUrl =>this.photoUrl=photoUrl);
  }
  login(){

    this.authService.login(this.model).subscribe(
      next=>this.alertify.success("connexion réussie"),
      error=>this.alertify.error(error),
      ()=>this.route.navigate(['/members'])
    )
  }
  loggedIn(){

   return this.authService.loggeIn();
  }
  loggedOut(){

    localStorage.removeItem('token');
    this.authService.decodedToken=null;
    localStorage.removeItem('user');
    this.authService.currentUser=null;
    this.alertify.message('déconexion réussi');
    this.route.navigate(['/home']);
  }

}
