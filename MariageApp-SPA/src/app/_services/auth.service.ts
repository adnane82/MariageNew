import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import {BehaviorSubject} from 'rxjs'

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  jwtHelper = new JwtHelperService();
  baseUrl = environment.apiUrl + 'auth/';
  decodedToken: any;
  currentUser:User;
  photoUrl = new BehaviorSubject<string>('../../assets/fonts/User.jpg');
  currentPhotoUrl = this.photoUrl.asObservable();


  constructor(private http: HttpClient) { }
  changeMemberPhoto(newPhotoUrl:string){
    this.photoUrl.next(newPhotoUrl);
  }
  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {

        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user',JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoUrl);

        }
      }


      )

    )

  }
  register(model: any) {

    return this.http.post(this.baseUrl + 'register', model);
  }
  loggeIn() {
    try {

      const token = localStorage.getItem('token');
      return !this.jwtHelper.isTokenExpired(token);

    } catch {
      false
    }

  }

}
