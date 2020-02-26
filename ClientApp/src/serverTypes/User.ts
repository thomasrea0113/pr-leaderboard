import Category from './Category';

export default interface User {
    userName: string;
    email: string;
    recommendations: Category[];
}
