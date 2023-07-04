import { NoteModel } from './note.model';

export class NoteAttachmentModel {
    id: number;
    displayName: string;
    fileName: string;
    noteId: number;
    note: NoteModel;

    constructor() {
        this.id = null;
        this.displayName = null;
        this.fileName = null;
        this.noteId = null;
        this.note = new NoteModel();
    }
}
