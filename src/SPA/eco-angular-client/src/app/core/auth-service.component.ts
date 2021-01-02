import { Injectable } from '@angular/core';
import { CoreModule } from './core.module';
import { UserManager, User } from 'oidc-client';
import { Constants } from '../constants';
import { Subject } from 'rxjs';

@Injectable({ providedIn: CoreModule })
export class AuthService {
    private _userManager: UserManager;
    private _user: User;
    private _loginChangedSubject = new Subject<boolean>();

    public loginChanged = this._loginChangedSubject.asObservable();

    constructor() {
        const stsSettings = {
            authority: Constants.stsAuthority,
            client_id: Constants.clientId,
            redirect_uri: `${Constants.clientRoot}signin-callback`,
            scope: 'openid profile econetwork',
            response_type: 'id_token token',
            post_logout_redirect_uri: `${Constants.clientRoot}signout-callback`
        };

        this._userManager = new UserManager(stsSettings);
    }

    public login() {
        return this._userManager.signinRedirect();
    }

    public isLoggedIn(): Promise<boolean> {
        return this._userManager.getUser().then(user => {
            const userCurrent = !!user && !user.expired;
            if (this._user !== user) {
                this._loginChangedSubject.next(userCurrent);
            }
            this._user = user;
            return userCurrent;
        });
    }

    public completeLogin() {
        return this._userManager.signinRedirectCallback().then(user => {
            this._user = user;
            this._loginChangedSubject.next(!!user && !user.expired);
            return user;
        });
    }

    public logout() {
        this._userManager.signoutRedirect();
    }

    public completeLogout() {
        this._user = null;
        return this._userManager.signoutRedirectCallback();
    }
}