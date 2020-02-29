import GenderValues from './GenderValues';

export default interface DivisionModel {
    id: string;
    name: string;
    gender?: GenderValues;
    ageLowerBound?: number;
    ageUpperBound?: number;
}
