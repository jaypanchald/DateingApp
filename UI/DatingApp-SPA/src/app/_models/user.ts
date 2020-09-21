import { Photo } from './photo';

export interface User {
    id: number;
    userName: string;
    knownAs: string;
    age: number;
    gender: string;
    lastActive: Date;
    city: string;
    country: string;
    intrests?: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photoUrl?: string;
    photos: Photo[];
}
