import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild ('editForm') editForm:NgModel
  user:User
  photoUrl:string;
  created:string;
  options :  Intl.DateTimeFormatOptions = {weekday : 'long' , year :'numeric' , month : 'long',day:'numeric'};
  @HostListener('window:beforeunload',['$event'])
  unLoadNotification($event:any){
    if(this.editForm.dirty){
      $event.returnValue=true;
    }
  }

  constructor(private route:ActivatedRoute,
     private alertify:AlertifyService,private userService: UserService, private authService:AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data=>{

      this.user=data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl=>this.photoUrl=photoUrl);
    this.created = new Date(this.user.created).toLocaleString('fr-EG', this.options);

  }
  updateUser() {
    
    
    this.userService.updateUser(this.authService.decodedToken.nameid,this.user).subscribe(()=>{
     this.alertify.success('Les modifications sont bien enregistrer');
     this.editForm.reset(this.user);
    },error=>this.alertify.error(error))
         
   }
   updateMainPhoto(photoUrl){
    this.user.photoUrl=photoUrl;
  }

}
