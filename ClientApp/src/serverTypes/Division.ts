import GenderValues from './GenderValues';

export default interface DivisionModel {
    name: string;
    gender?: GenderValues;
    ageLowerBound?: number;
    ageUpperBound?: number;
}
