/**
 * Types that are meant to interface directly with the dotnet core server-side application
 */

export enum GenderValues {
    Male = 'Male',
    Female = 'Female',
    Other = 'Other',
}

export enum Unit {
    Kilograms = 'Kilograms',
    Seconds = 'Seconds',
    Meters = 'Meters',
}

export interface Score {
    id: string;
    ApprovedDate?: Date;
    board: Leaderboard;
    user: User;
    createdDate: Date;
    value: number;
}

export interface User {
    id: string;
    userName: string;
    gender?: GenderValues;
    email?: string;
    isActive: boolean;
    isAdmin: boolean;
    interests: Category[];
    leaderboards: Leaderboard[];
}

export interface Leaderboard {
    id: string;
    name: string;
    iconUrl?: string;
    uom: Uom;
    division: Division;
    weightClass?: WeightClass;
    slug: string;
    viewUrl?: string;
    joinUrl?: string;
}

export interface Division {
    id: string;
    name: string;
    slug: string;
    categories: Category[];
    gender?: GenderValues;
    ageLowerBound?: number;
    ageUpperBound?: number;
}

export interface WeightClass {
    weightLowerBound?: number;
    weightUpperBound?: number;
    range: string;
}

export interface Category {
    name: string;
}

export interface UserView extends Leaderboard {
    isMember: boolean;
    isRecommended: boolean;
}

export interface Uom {
    unit: Unit;
}
