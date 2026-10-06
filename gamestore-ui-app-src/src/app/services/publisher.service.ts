import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { ConfigService } from './config.service';
import { Publisher, CreatePublisherRequest, UpdatePublisherRequest } from '../models/publisher.model';

@Injectable({
  providedIn: 'root'
})
export class PublisherService {
  private mockPublishers: Publisher[] = [
    { id: '77c06fc6-f8a1-46f9-af03-2793e500112c', companyName: 'Electronic Arts', homePage: 'https://www.ea.com', description: 'American video game company' },
    { id: '0f7f61ac-babe-423d-a6fe-a5a6c842eacd', companyName: 'Test Name', homePage: 'https://localhost:7091/swagger/index.html', description: 'Some text' },
    { id: '4ca4f3b7-7e23-45f6-8c43-bcde64a51fac', companyName: 'Valve', homePage: 'https://www.valvesoftware.com', description: 'Creators of Steam and Half-Life' },
    { id: '5bc5d6a2-6f91-4d3f-91ab-64213d8e578c', companyName: 'Bethesda', homePage: 'https://bethesda.net', description: 'Makers of Skyrim and Fallout' },
    { id: '1c3a64b9-9142-4f3b-823c-7c4a1653d8a1', companyName: 'CD Projekt Red', homePage: 'https://cdprojekt.com', description: 'Witcher and Cyberpunk developers' },
    { id: 'a1110001-0001-4001-8001-000000000001', companyName: 'Nintendo', homePage: 'https://www.nintendo.com', description: 'Japanese gaming giant' },
    { id: 'a1110002-0002-4002-8002-000000000002', companyName: 'Sony Interactive', homePage: 'https://www.playstation.com', description: 'PlayStation maker' },
    { id: 'a1110003-0003-4003-8003-000000000003', companyName: 'Ubisoft', homePage: 'https://www.ubisoft.com', description: "French publisher of Assassin's Creed" },
    { id: 'a1110004-0004-4004-8004-000000000004', companyName: 'Activision', homePage: 'https://www.activision.com', description: 'Call of Duty publisher' },
    { id: 'a1110005-0005-4005-8005-000000000005', companyName: 'Square Enix', homePage: 'https://www.square-enix.com', description: 'Final Fantasy developers' },
    { id: 'a1110006-0006-4006-8006-000000000006', companyName: 'Capcom', homePage: 'https://www.capcom.com', description: 'Resident Evil and Monster Hunter' },
    { id: 'a1110007-0007-4007-8007-000000000007', companyName: 'Rockstar Games', homePage: 'https://www.rockstargames.com', description: 'GTA and Red Dead developers' },
    { id: 'a1110008-0008-4008-8008-000000000008', companyName: 'FromSoftware', homePage: 'https://www.fromsoftware.jp', description: 'Dark Souls and Elden Ring creators' },
    { id: 'a1110009-0009-4009-8009-000000000009', companyName: 'Bungie', homePage: 'https://www.bungie.net', description: 'Destiny developers' },
    { id: 'a1110010-0010-4010-8010-000000000010', companyName: 'Epic Games', homePage: 'https://www.epicgames.com', description: 'Fortnite and Unreal Engine' },
    { id: 'a1110011-0011-4011-8011-000000000011', companyName: 'Sega', homePage: 'https://www.sega.com', description: 'Sonic and Yakuza publisher' },
    { id: 'a1110012-0012-4012-8012-000000000012', companyName: 'Bandai Namco', homePage: 'https://www.bandainamcoent.com', description: 'Tekken and Elden Ring publisher' },
    { id: 'a1110013-0013-4013-8013-000000000013', companyName: 'Konami', homePage: 'https://www.konami.com', description: 'Metal Gear Solid publisher' },
    { id: 'a1110014-0014-4014-8014-000000000014', companyName: 'Take-Two', homePage: 'https://www.take2games.com', description: 'Parent of Rockstar and 2K' },
    { id: 'a1110015-0015-4015-8015-000000000015', companyName: 'Xbox Game Studios', homePage: 'https://www.xbox.com', description: 'Microsoft gaming division' },
    { id: 'a1110016-0016-4016-8016-000000000016', companyName: 'Naughty Dog', homePage: 'https://www.naughtydog.com', description: 'The Last of Us and Uncharted' },
    { id: 'a1110017-0017-4017-8017-000000000017', companyName: 'Insomniac Games', homePage: 'https://insomniac.games', description: 'Spider-Man PS developers' },
    { id: 'a1110018-0018-4018-8018-000000000018', companyName: 'Devolver Digital', homePage: 'https://www.devolverdigital.com', description: 'Indie game publisher' },
    { id: 'a1110019-0019-4019-8019-000000000019', companyName: 'Supergiant Games', homePage: 'https://www.supergiantgames.com', description: 'Hades and Bastion creators' },
    { id: 'a1110020-0020-4020-8020-000000000020', companyName: 'Riot Games', homePage: 'https://www.riotgames.com', description: 'League of Legends developer' }
  ];

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getPublishers(): Observable<Publisher[]> {
    const url = this.configService.getApiUrl('publishersApiUrl');
    return this.http.get<Publisher[]>(url).pipe(
      catchError(() => of(this.mockPublishers))
    );
  }

  getPublisherByCompanyName(companyname: string): Observable<Publisher> {
    const url = this.configService.getApiUrl('publisherApiUrl', { companyname });
    return this.http.get<Publisher>(url).pipe(
      catchError(() => of(this.mockPublishers.find(p => p.companyName.toLowerCase() === companyname.toLowerCase()) || this.mockPublishers[0]))
    );
  }

  createPublisher(request: CreatePublisherRequest): Observable<any> {
    const url = this.configService.getApiUrl('addPublisherApiUrl');
    return this.http.post(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  updatePublisher(request: UpdatePublisherRequest): Observable<any> {
    const url = this.configService.getApiUrl('updatePublisherApiUrl');
    return this.http.put(url, request).pipe(
      catchError(() => of({ success: true }))
    );
  }

  deletePublisher(id: string): Observable<any> {
    const url = this.configService.getApiUrl('deletePublisherApiUrl', { id });
    return this.http.delete(url).pipe(
      catchError(() => of({ success: true }))
    );
  }
}
