import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AutocompleteService {
  private apiUrl = 'https://api.radar.io/v1/search/autocomplete';
  private apiKey = 'prj_test_pk_7867c6b3fd10a0a6fc05bef60126b5c7e832cdad';

  constructor(private http: HttpClient) { }

  getAutocompleteSuggestions(query: string): Observable<any> {
    const url = `${this.apiUrl}?query=${encodeURIComponent(query)}`;
    const headers = new HttpHeaders().set('Authorization', `${this.apiKey}`);
    return this.http.get(url, { headers });
  }
}
