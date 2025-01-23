export class Credentials {

    constructor(
        public userName?: string,
        public password?: string
    ) {
    }

}

export class ChangePasswordModel {

    constructor(
        public currentPassword?: string,
        public newPassword?: string,
        public confirmNewPassword?: string
    ) {

    }

    toJSON() {
        return {
            currentPassword: this.currentPassword,
            newPassword: this.newPassword
        };
    }

}

export class User {

    constructor(
        readonly id: number,
        readonly userName: string,
        readonly enabled: boolean,
        readonly role: Role,
        readonly accessToken: string,
        readonly operatorId: number
    ) {
    }

    static build(obj: User) {
        return new User(obj.id, obj.userName, obj.enabled, obj.role, obj.accessToken, obj.operatorId);
    }

}

export enum Role {

    Administrator,
    Operator,
    Customer

}

export class UpdateUserRequest {

    constructor(
        public userName: string,
        public enabled: boolean,
        public role: Role,
        public password: string
    ) {
    }

    static empty() {
        return new UpdateUserRequest(null, true, Role.Operator, null);
    }

    static fromUser(user: User) {
        return new UpdateUserRequest(user.userName, user.enabled, user.role, null);
    }

}

export interface IUser {

    readonly id: number;
    readonly userName: string;
    readonly enabled: boolean;
    readonly role: Role;

}
