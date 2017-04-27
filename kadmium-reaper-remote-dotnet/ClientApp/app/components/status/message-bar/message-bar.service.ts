import { Injectable } from '@angular/core';
import { Response } from "@angular/http";
import { MessageBarComponent } from "./message-bar.component";
import { StatusCode } from "../status";

@Injectable()
export class MessageBarService
{
    public component: MessageBarComponent;
    constructor()
    {
        this.component = null;
    }

    public add(statusCode: StatusCode, message: any): void
    {
        if (statusCode == "Error")
        {
            if (message instanceof Response)
            {
                this.component.addError(message as Response);
            }
            else
            {
                console.error(message);
            }
        }
        else
        {
            this.component.add(statusCode, message);
        }

    }
}