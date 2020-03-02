export interface WeightClassModel {
    weightLowerBound?: number;
    weightUpperBound?: number;
}

export interface DivisionModel {
    id: string;
    name: string;
    gender?: GenderValues;
    ageLowerBound?: number;
    ageUpperBound?: number;
}

export interface CategoryModel {
    id: string;
    name: string;
}

export interface Leaderboard {
    public id: string;
    public name: string;
    public uom: UnitOfMeasure;
    public division: Division;
    public weightClass: WeightClass;
}

export interface UserView extends Leaderboard {
    public isMember: boolean;
    public isRecommended: boolean;
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
