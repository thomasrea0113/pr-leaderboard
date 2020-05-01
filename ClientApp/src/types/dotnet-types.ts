/**
 * Types that are meant to interface directly with the dotnet core server-side application
 */

export enum GenderValues {
    Male,
    Female,
    Other,
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

export enum Unit {
    Kilograms,
    Seconds,
    Meters,
}
export interface KeyValuePair {
    key: string;
    value: string;
}

export interface WeightClass {
    weightLowerBound?: number;
    weightUpperBound?: number;
}

export interface Division {
    id: string;
    name: string;
    categories: Category[];
    icon?: string;
    gender?: keyof typeof GenderValues;
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
    slug: string;
}

export interface UserView extends Leaderboard {
    isMember: boolean;
    isRecommended: boolean;
    viewUrl?: string;
    joinUrl?: string;
}

export interface User {
    userName: string;
    email?: string;
    isAdmin: boolean;
    interests: Category[];
    leaderboards: Leaderboard[];
}

export interface UnitOfMeasure {
    unit: keyof typeof Unit;
}

export interface Score {
    id: string;
    isApproved: boolean;
    boardId: string;
    userId: string;
    value: number;
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
