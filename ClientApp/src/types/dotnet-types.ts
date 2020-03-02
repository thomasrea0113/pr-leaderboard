/**
 * Types that are meant to interface directly with the dotnet core server-side application
 */

export interface WeightClass {
    weightLowerBound?: number;
    weightUpperBound?: number;
}

export interface Division {
    id: string;
    name: string;
    gender?: GenderValues;
    ageLowerBound?: number;
    ageUpperBound?: number;
}

export interface Category {
    id: string;
    name: string;
}

export interface Leaderboard {
    id: string;
    name: string;
    uom: UnitOfMeasure;
    division: Division;
    weightClass: WeightClass;
}

export interface UserView extends Leaderboard {
    isMember: boolean;
    isRecommended: boolean;
}

export interface User {
    userName: string;
    email: string;
    interests: Category[];
    leaderboards: Leaderboard[];
}

export interface UnitOfMeasure {
    unit: string;
}

// eslint-disable-next-line import/prefer-default-export
export enum GenderValues {
    Male = 'Male',
    Female = 'Female',
    Other = 'Other',
}
