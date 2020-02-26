import Category from './Category';

export default interface Division {
    id: string;
    gender: string;
    name: string;
    ageLowerBound: number | null;
    ageUpperBound: number | null;
    categories: Category[];
}
