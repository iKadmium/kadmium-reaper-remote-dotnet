import { Injectable } from '@angular/core';

import { SongData, Song } from "../song";
import { UrlService } from "./url.service";
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class SongService
{
    constructor(private http: HttpClient, private urlService: UrlService) { }

    public getSongs(): Promise<Song[]>
    {
        return this.http
            .get<SongData[]>(this.urlService.getUrl("Song", null, null))
            .toPromise()
            .then(response =>
            {
                return response.map((value: SongData) => new Song().load(value));
            });
    }

    public getSong(id: number): Promise<Song>
    {
        return this.http
            .get<SongData>(this.urlService.getUrl("Song", null, id))
            .toPromise()
            .then(response =>
            {
                let song = new Song();
                song.load(response);
                return song;
            });
    }

    public postSong(song: Song): Promise<number>
    {
        return this.http.post<number>(this.urlService.getUrl("Song", null, null), song.serialize())
            .toPromise();
    }

    public putSong(song: Song): Promise<void>
    {
        return this.http.put<void>(this.urlService.getUrl("Song", null, song.id), song.serialize())
            .toPromise();
    }

    public removeSong(id: number): Promise<void>
    {
        return this.http.delete<void>(this.urlService.getUrl("Song", null, id))
            .toPromise();
    }
}