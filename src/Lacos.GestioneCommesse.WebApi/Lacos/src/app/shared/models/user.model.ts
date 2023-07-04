import { ActivityModel } from './activity.model';
import { RoleEnum } from '../enums/role.enum';

export class UserModel {
    userName: string;
    role: RoleEnum;
    emailAddress: string;
    colorHex: string;
    name: string;
    surname: string;
    activities: ActivityModel[];
}
