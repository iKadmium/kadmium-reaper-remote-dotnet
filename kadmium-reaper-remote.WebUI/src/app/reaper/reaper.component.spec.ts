import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReaperComponent } from './reaper.component';
import { ReaperService } from "../reaper.service";

describe('ReaperComponent', () =>
{
    let component: ReaperComponent;
    let fixture: ComponentFixture<ReaperComponent>;

    beforeEach(async(() =>
    {
        TestBed.configureTestingModule({
            declarations: [ReaperComponent]
        }).overrideComponent(ReaperComponent, {
            set: {
                providers: [
                    { provide: ReaperService, useValue: jasmine.createSpyObj<ReaperService>({ runCommand: Promise.resolve(null) }) }
                ]
            }
        }).compileComponents();
    }));

    beforeEach(() =>
    {
        fixture = TestBed.createComponent(ReaperComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () =>
    {
        expect(component).toBeTruthy();
    });
});
