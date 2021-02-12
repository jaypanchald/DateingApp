export interface Message {
    id: number;
    senderId: number;
    senderUsername: string;
    senderKnownAs: string;
    senderPhotoUrl: string;
    recipientId: Int16Array;
    recipientUsername: string;
    recipientKnownAs: string;
    recipientPhotoUrl: string;
    content: string;
    isRead: boolean;
    dateRead?: Date;
    messageSent: Date;
}
