import {Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Injectable({
  providedIn: 'root',
})
export class AppService {
  
  constructor(private http: HttpClient){}
  url = 'http://localhost:5131/patients';
  //url = 'https://apim-ehr-poc.azure-api.net/api/patients';

  async getPatients(): Promise<Patient[]> {
      try {
          const data = await this.http.get<Patient[]>(this.url).toPromise();
          return data ?? [];
      } catch (error) {
          console.error('Error:', error);
          return [];
      }
  }
}

export interface Patient {
    id: string;
    name: string;
    age: number;
    gender: string;
    address: Address;
    medicalHistory: MedicalHistory[];
    laboratoryTests: LaboratoryTest[];
  }
  
  export interface Address {
    street: string;
    city: string;
    state: string;
    zipCode: string;
    country: string;
  }

  export interface MedicalHistory {
    condition: string;
    diagnosisDate: Date;
    treatment: string;
  }
  
  export interface LaboratoryTest {
    testName: string;
    testDate: Date;
    result: string;
  }
