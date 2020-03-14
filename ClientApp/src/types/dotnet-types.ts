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
    categories: Category[];
    icon?: string;
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
    iconUrl: string;
    name: string;
    uom: UnitOfMeasure;
    division: Division;
    weightClass: WeightClass;
}

export interface UserView extends Leaderboard {
    isMember: boolean;
    isRecommended: boolean;
    viewUrl?: string;
    joinUrl?: string;
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

export interface Score {
    isApproved: boolean;
    leaderboardId: string;
    userId: string;
    value: number;
}

export enum BootstrapColorClass {
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Light,
    Dark,
    Link,
}

export enum FontawesomeIcon {
    Go = 'fas fa-chevron-right',
    User = 'fas fa-user',
}

export interface Link {
    // The server returns the string key of the enum (server and client enums have the same keys)
    // so this is the key of the enum, not the value
    addon?: keyof typeof FontawesomeIcon;
    label: string;
    className?: keyof typeof BootstrapColorClass;
    url: string;
}

export interface Featured {
    title: string;
    description: string;
    image: string;
    links: Link[];
}
