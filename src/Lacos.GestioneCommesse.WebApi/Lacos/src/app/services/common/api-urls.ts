import { environment } from '../../../environments/environment';

export class ApiUrls {

    static readonly baseUrl = environment.baseUrl;
    static readonly baseApiUrl = `${ApiUrls.baseUrl}/api`;
    static readonly baseAttachmentsUrl = `${ApiUrls.baseUrl}/attachments`;

}
